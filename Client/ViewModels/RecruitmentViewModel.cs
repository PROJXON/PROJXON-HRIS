using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Client.Services;
using Client.Utils.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Client.ViewModels;

/// <summary>
/// ViewModel for the Recruitment view
/// Displays a Kanban-style board for tracking candidates through the hiring pipeline
/// </summary>
public partial class RecruitmentViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    #region Sidebar User Profile

    [ObservableProperty]
    private string _userName = "John Smith";

    [ObservableProperty]
    private string _userRole = "HR Manager";

    #endregion

    #region Pipeline Stages

    [ObservableProperty]
    private ObservableCollection<RecruitmentStageViewModel> _stages = new();

    #endregion

    #region Add Candidate Dialog

    [ObservableProperty]
    private bool _isAddCandidateDialogOpen;

    [ObservableProperty]
    private string _newCandidateFullName = string.Empty;

    [ObservableProperty]
    private string _newCandidateEmail = string.Empty;

    [ObservableProperty]
    private string _newCandidatePosition = string.Empty;

    [ObservableProperty]
    private string _newCandidateLocation = string.Empty;

    #endregion

    #region Move to Stage Dialog

    [ObservableProperty]
    private bool _isMoveToStageDialogOpen;

    [ObservableProperty]
    private CandidateViewModel? _selectedCandidateForMove;

    [ObservableProperty]
    private ObservableCollection<StageOption> _availableStages = new();

    [ObservableProperty]
    private string _selectedStageForMove = string.Empty;

    #endregion

    public RecruitmentViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        InitializeStages();
        LoadMockData();
    }

    // Parameterless constructor for design-time support
    public RecruitmentViewModel() : this(null!)
    {
    }

    private void InitializeStages()
    {
        Stages = new ObservableCollection<RecruitmentStageViewModel>
        {
            new() { StageName = "Application Review", StageId = "application_review" },
            new() { StageName = "Technical Assessment", StageId = "technical_assessment" },
            new() { StageName = "Interview", StageId = "interview" },
            new() { StageName = "Offer", StageId = "offer" },
            new() { StageName = "Hired", StageId = "hired" },
            new() { StageName = "Rejected", StageId = "rejected" }
        };

        AvailableStages = new ObservableCollection<StageOption>
        {
            new() { StageId = "application_review", StageName = "Application Review" },
            new() { StageId = "technical_assessment", StageName = "Technical Assessment" },
            new() { StageId = "interview", StageName = "Interview" },
            new() { StageId = "offer", StageName = "Offer" },
            new() { StageId = "hired", StageName = "Hired" },
            new() { StageId = "rejected", StageName = "Rejected" }
        };
    }

    private void LoadMockData()
    {
        // Application Review
        var applicationReview = Stages.First(s => s.StageId == "application_review");
        applicationReview.Candidates.Add(new CandidateViewModel
        {
            Id = 1,
            FullName = "James Rodriguez",
            Position = "Backend Developer",
            Location = "Austin, TX",
            AppliedDate = new DateTime(2025, 10, 9),
            Email = "james.rodriguez@email.com",
            CurrentStageId = "application_review"
        });

        // Technical Assessment
        var technicalAssessment = Stages.First(s => s.StageId == "technical_assessment");
        technicalAssessment.Candidates.Add(new CandidateViewModel
        {
            Id = 2,
            FullName = "Emma Thompson",
            Position = "Product Manager",
            Location = "Seattle, WA",
            AppliedDate = new DateTime(2025, 10, 4),
            Email = "emma.thompson@email.com",
            CurrentStageId = "technical_assessment"
        });

        // Interview
        var interview = Stages.First(s => s.StageId == "interview");
        interview.Candidates.Add(new CandidateViewModel
        {
            Id = 3,
            FullName = "Michael Chen",
            Position = "Senior Frontend Developer",
            Location = "New York, NY",
            AppliedDate = new DateTime(2025, 9, 30),
            Email = "michael.chen@email.com",
            CurrentStageId = "interview"
        });
        interview.Candidates.Add(new CandidateViewModel
        {
            Id = 4,
            FullName = "David Kim",
            Position = "DevOps Engineer",
            Location = "Boston, MA",
            AppliedDate = new DateTime(2025, 10, 7),
            Email = "david.kim@email.com",
            CurrentStageId = "interview"
        });

        // Offer
        var offer = Stages.First(s => s.StageId == "offer");
        offer.Candidates.Add(new CandidateViewModel
        {
            Id = 5,
            FullName = "Sarah Williams",
            Position = "UX Designer",
            Location = "San Francisco, CA",
            AppliedDate = new DateTime(2025, 9, 27),
            Email = "sarah.williams@email.com",
            CurrentStageId = "offer"
        });

        // Update candidate counts
        foreach (var stage in Stages)
        {
            stage.UpdateCandidateCount();
        }
    }

    #region Add Candidate Commands

    [RelayCommand]
    private void OpenAddCandidateDialog()
    {
        NewCandidateFullName = string.Empty;
        NewCandidateEmail = string.Empty;
        NewCandidatePosition = string.Empty;
        NewCandidateLocation = string.Empty;
        IsAddCandidateDialogOpen = true;
    }

    [RelayCommand]
    private void CloseAddCandidateDialog()
    {
        IsAddCandidateDialogOpen = false;
    }

    [RelayCommand]
    private void AddCandidate()
    {
        if (string.IsNullOrWhiteSpace(NewCandidateFullName))
            return;

        var newCandidate = new CandidateViewModel
        {
            Id = Stages.SelectMany(s => s.Candidates).Max(c => c.Id) + 1,
            FullName = NewCandidateFullName,
            Email = NewCandidateEmail,
            Position = NewCandidatePosition,
            Location = NewCandidateLocation,
            AppliedDate = DateTime.Today,
            CurrentStageId = "application_review"
        };

        var applicationReviewStage = Stages.First(s => s.StageId == "application_review");
        applicationReviewStage.Candidates.Add(newCandidate);
        applicationReviewStage.UpdateCandidateCount();

        IsAddCandidateDialogOpen = false;
    }

    #endregion

    #region Move to Stage Commands

    [RelayCommand]
    private void OpenMoveToStageDialog(CandidateViewModel? candidate)
    {
        if (candidate == null) return;

        SelectedCandidateForMove = candidate;
        SelectedStageForMove = candidate.CurrentStageId;
        
        // Update selection state
        foreach (var stage in AvailableStages)
        {
            stage.IsSelected = stage.StageId == candidate.CurrentStageId;
        }
        
        IsMoveToStageDialogOpen = true;
    }

    [RelayCommand]
    private void CloseMoveToStageDialog()
    {
        IsMoveToStageDialogOpen = false;
        SelectedCandidateForMove = null;
    }

    [RelayCommand]
    private void SelectStage(StageOption? stageOption)
    {
        if (stageOption == null || SelectedCandidateForMove == null) return;

        // Update selection state
        foreach (var stage in AvailableStages)
        {
            stage.IsSelected = stage.StageId == stageOption.StageId;
        }

        // Move candidate to new stage
        var currentStage = Stages.First(s => s.StageId == SelectedCandidateForMove.CurrentStageId);
        var newStage = Stages.First(s => s.StageId == stageOption.StageId);

        if (currentStage.StageId != newStage.StageId)
        {
            currentStage.Candidates.Remove(SelectedCandidateForMove);
            SelectedCandidateForMove.CurrentStageId = stageOption.StageId;
            newStage.Candidates.Add(SelectedCandidateForMove);

            currentStage.UpdateCandidateCount();
            newStage.UpdateCandidateCount();
        }

        IsMoveToStageDialogOpen = false;
        SelectedCandidateForMove = null;
    }

    #endregion

    #region Navigation Commands

    [RelayCommand]
    private async Task NavigateToDashboard()
    {
        await _navigationService.NavigateTo(ViewModelType.HRDashboard);
    }

    [RelayCommand]
    private async Task NavigateToProfile()
    {
        await _navigationService.NavigateTo(ViewModelType.Profile);
    }

    [RelayCommand]
    private async Task NavigateToTimeOff()
    {
        // TODO: Navigate to time off view when implemented
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task NavigateToAttendance()
    {
        await _navigationService.NavigateTo(ViewModelType.Attendance);
    }

    [RelayCommand]
    private async Task NavigateToEmployees()
    {
        await _navigationService.NavigateTo(ViewModelType.Employees);
    }

    [RelayCommand]
    private async Task NavigateToRecruitment()
    {
        // Already on recruitment page
        await Task.CompletedTask;
    }

    #endregion

    public override async Task OnNavigatedToAsync()
    {
        // TODO: Load actual recruitment data from API
        await base.OnNavigatedToAsync();
    }
}

/// <summary>
/// Represents a stage in the recruitment pipeline
/// </summary>
public partial class RecruitmentStageViewModel : ObservableObject
{
    [ObservableProperty]
    private string _stageId = string.Empty;

    [ObservableProperty]
    private string _stageName = string.Empty;

    [ObservableProperty]
    private ObservableCollection<CandidateViewModel> _candidates = new();

    [ObservableProperty]
    private string _candidateCountText = "0 candidates";

    [ObservableProperty]
    private bool _hasNoCandidates;

    public void UpdateCandidateCount()
    {
        var count = Candidates.Count;
        CandidateCountText = count == 1 ? "1 candidate" : $"{count} candidates";
        HasNoCandidates = count == 0;
    }
}

/// <summary>
/// Represents a candidate in the recruitment pipeline
/// </summary>
public partial class CandidateViewModel : ObservableObject
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _fullName = string.Empty;

    [ObservableProperty]
    private string _position = string.Empty;

    [ObservableProperty]
    private string _location = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private DateTime _appliedDate;

    [ObservableProperty]
    private string _currentStageId = string.Empty;

    [ObservableProperty]
    private bool _isHovered;

    public string Initial => !string.IsNullOrEmpty(FullName) ? FullName[0].ToString().ToUpper() : "?";
    
    public string AppliedDateDisplay => $"Applied {AppliedDate:M/d/yyyy}";
}

/// <summary>
/// Represents a stage option in the move dialog
/// </summary>
public partial class StageOption : ObservableObject
{
    [ObservableProperty]
    private string _stageId = string.Empty;

    [ObservableProperty]
    private string _stageName = string.Empty;

    [ObservableProperty]
    private bool _isSelected;
}
