using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Client.Services;
using Client.Utils.Enums;
using Client.Utils.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.CandidateManagement.Requests;
using Shared.CandidateManagement.Responses;

namespace Client.ViewModels;

/// <summary>
/// ViewModel for the Recruitment view
/// Displays a Kanban-style board for tracking candidates through the hiring pipeline
/// </summary>
public partial class RecruitmentViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IApiClient _apiClient;

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

    [ObservableProperty]
    private bool _isLoading;

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

    #region Hire Confirmation

    [ObservableProperty]
    private bool _isHireConfirmationOpen;

    [ObservableProperty]
    private CandidateViewModel? _candidateToHire;

    #endregion

    public RecruitmentViewModel(INavigationService navigationService, IApiClient apiClient)
    {
        _navigationService = navigationService;
        _apiClient = apiClient;
        InitializeStages();
    }

    // Parameterless constructor for design-time support
    public RecruitmentViewModel() : this(null!, null!)
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

    private async Task LoadCandidatesAsync()
    {
        var result = await _apiClient.GetAllAsync<IEnumerable<CandidateResponse>>("api/candidate");
        if (result.IsSuccess && result.Data != null)
        {
            foreach (var stage in Stages) stage.Candidates.Clear();

            foreach (var c in result.Data)
            {
                // Normalize Backend Status to Frontend Stage ID
                string mappedStageId = "application_review"; // Default

                if (!string.IsNullOrEmpty(c.Status))
                {
                    var normalized = c.Status.Trim();

                    if (normalized.Equals("Applied", StringComparison.OrdinalIgnoreCase))
                        mappedStageId = "application_review";
                    else if (normalized.Equals("Technical Assessment", StringComparison.OrdinalIgnoreCase))
                        mappedStageId = "technical_assessment";
                    else if (normalized.Equals("Interview", StringComparison.OrdinalIgnoreCase))
                        mappedStageId = "interview";
                    else if (normalized.Equals("Offer", StringComparison.OrdinalIgnoreCase))
                        mappedStageId = "offer";
                    else if (normalized.Equals("Hired", StringComparison.OrdinalIgnoreCase))
                        mappedStageId = "hired";
                    else if (normalized.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
                        mappedStageId = "rejected";
                }

                var vm = new CandidateViewModel
                {
                    Id = c.Id,
                    FullName = $"{c.FirstName} {c.LastName}",
                    Email = c.Email,
                    Position = c.JobAppliedFor,
                    Location = c.Location,
                    AppliedDate = c.AppliedDate,
                    CurrentStageId = mappedStageId
                };

                var stage = Stages.FirstOrDefault(s => s.StageId == mappedStageId);
                stage?.Candidates.Add(vm);
            }

            foreach (var stage in Stages) stage.UpdateCandidateCount();
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
    private async Task AddCandidate()
    {
        if (string.IsNullOrWhiteSpace(NewCandidateFullName))
            return;

        // Split name for API
        var names = NewCandidateFullName.Trim().Split(' ', 2);
        var firstName = names[0];
        var lastName = names.Length > 1 ? names[1] : ".";

        var request = new CreateCandidateRequest
        {
            FirstName = firstName,
            LastName = lastName,
            Email = NewCandidateEmail,
            JobAppliedFor = NewCandidatePosition,
            Location = NewCandidateLocation,
            Phone = "N/A" // Default or add field to UI
        };

        IsLoading = true;
        try
        {
            // Save to Backend
            var response = await _apiClient.PostAsync<CandidateResponse>("api/candidate", request);

            if (response.IsSuccess && response.Data != null)
            {
                // Add to UI using the REAL ID from the database
                var newCandidate = new CandidateViewModel
                {
                    Id = response.Data.Id, // CRITICAL: This links UI to DB
                    FullName = $"{response.Data.FirstName} {response.Data.LastName}",
                    Email = response.Data.Email,
                    Position = response.Data.JobAppliedFor,
                    Location = response.Data.Location,
                    AppliedDate = response.Data.AppliedDate,
                    CurrentStageId = "application_review"
                };

                var applicationReviewStage = Stages.First(s => s.StageId == "application_review");
                applicationReviewStage.Candidates.Add(newCandidate);
                applicationReviewStage.UpdateCandidateCount();

                IsAddCandidateDialogOpen = false;

                // Clear fields
                NewCandidateFullName = string.Empty;
                NewCandidateEmail = string.Empty;
                NewCandidatePosition = string.Empty;
                NewCandidateLocation = string.Empty;
            }
            else
            {
                // Handle error (optional: add an ErrorMessage property to bind to in UI)
                Console.WriteLine($"Failed to add candidate: {response.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
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
    private async Task SelectStage(StageOption? stageOption)
    {
        if (stageOption == null || SelectedCandidateForMove == null) return;

        // Intercept "Hired" selection - trigger hire confirmation popup instead
        if (stageOption.StageId.Equals("hired", StringComparison.OrdinalIgnoreCase))
        {
            // Close the move dialog
            IsMoveToStageDialogOpen = false;
            // Trigger the hire confirmation popup instead
            InitiateHire(SelectedCandidateForMove);
            // Clear selection
            SelectedCandidateForMove = null;
            return;
        }

        // Update selection state
        foreach (var stage in AvailableStages)
        {
            stage.IsSelected = stage.StageId == stageOption.StageId;
        }

        // Find Current Stage (Safe Lookup)
        var currentStage = Stages.FirstOrDefault(s => s.StageId == SelectedCandidateForMove.CurrentStageId);
        var newStage = Stages.FirstOrDefault(s => s.StageId == stageOption.StageId);

        if (currentStage != null && newStage != null && currentStage.StageId != newStage.StageId)
        {
            // Move in UI immediately
            currentStage.Candidates.Remove(SelectedCandidateForMove);
            SelectedCandidateForMove.CurrentStageId = stageOption.StageId;
            newStage.Candidates.Add(SelectedCandidateForMove);

            currentStage.UpdateCandidateCount();
            newStage.UpdateCandidateCount();

            // Map ID back to readable Status string for DB
            string dbStatus = stageOption.StageName;
            if (stageOption.StageId == "application_review") dbStatus = "Applied";

            try
            {
                // We send the status string as a JSON string
                await _apiClient.PutAsync<object>($"api/candidate/{SelectedCandidateForMove.Id}/status", dbStatus);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save stage move: {ex.Message}");
            }
        }

        IsMoveToStageDialogOpen = false;
        SelectedCandidateForMove = null;
    }

    #endregion

    #region Hire Confirmation Commands

    [RelayCommand]
    private void InitiateHire(CandidateViewModel candidate)
    {
        CandidateToHire = candidate;
        IsHireConfirmationOpen = true;
    }

    [RelayCommand]
    private void CancelHire()
    {
        IsHireConfirmationOpen = false;
        CandidateToHire = null;
    }

    [RelayCommand]
    private async Task ConfirmHire()
    {
        if (CandidateToHire == null) return;

        try 
        {
            // Execute Backend Call
            var result = await _apiClient.PostAsync<object>($"api/candidate/{CandidateToHire.Id}/hire", null);

            // Update UI if Successful
            if (result.IsSuccess)
            {
                // ROBUST FIND: Find the stage that actually holds this candidate object right now
                var sourceStage = Stages.FirstOrDefault(s => s.Candidates.Contains(CandidateToHire));
                
                // Find Target
                var targetStage = Stages.FirstOrDefault(s => s.StageId == "hired");

                // Move the card
                if (sourceStage != null && targetStage != null && sourceStage != targetStage)
                {
                    sourceStage.Candidates.Remove(CandidateToHire);
                    
                    CandidateToHire.CurrentStageId = "hired"; // Update the data model
                    targetStage.Candidates.Add(CandidateToHire); // Add to new UI column

                    // Refresh counters
                    sourceStage.UpdateCandidateCount();
                    targetStage.UpdateCandidateCount();
                }
            }
            else 
            {
                Console.WriteLine($"Hire failed: {result.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ConfirmHire: {ex.Message}");
        }
        finally
        {
            // Always close the dialog
            IsHireConfirmationOpen = false;
            CandidateToHire = null;
        }
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

    [RelayCommand]
    private async Task NavigateToForms()
    {
        await _navigationService.NavigateTo(ViewModelType.Forms);
    }

    #endregion

    public override async Task OnNavigatedToAsync()
    {
        await LoadCandidatesAsync();
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