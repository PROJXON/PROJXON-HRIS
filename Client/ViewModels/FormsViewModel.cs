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
/// ViewModel for the Forms view
/// Displays a list of forms/surveys with their status and response statistics
/// </summary>
public partial class FormsViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    // Shared Sidebar
    public SidebarViewModel Sidebar { get; }

    #region Forms Data

    [ObservableProperty]
    private ObservableCollection<FormItemViewModel> _forms = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    #endregion

    public FormsViewModel(INavigationService navigationService, SidebarViewModel sidebarViewModel)
    {
        _navigationService = navigationService;
        Sidebar = sidebarViewModel;
        
        LoadMockData();
    }

    // Parameterless constructor for design-time support
    public FormsViewModel() : this(null!, null!)
    {
    }

    private void LoadMockData()
    {
        Forms = new ObservableCollection<FormItemViewModel>
        {
            new()
            {
                Id = 1,
                Title = "Employee Satisfaction Survey Q4 2025",
                Description = "Quarterly employee satisfaction and engagement survey",
                Status = FormStatus.Active,
                CreatedDate = new DateTime(2025, 9, 30),
                ResponseCount = 32,
                TotalRecipients = 48
            },
            new()
            {
                Id = 2,
                Title = "New Hire Onboarding Feedback",
                Description = "Feedback on the onboarding process for new employees",
                Status = FormStatus.Completed,
                CreatedDate = new DateTime(2025, 9, 14),
                ResponseCount = 8,
                TotalRecipients = 8
            },
            new()
            {
                Id = 3,
                Title = "Remote Work Policy Feedback",
                Description = "Gathering feedback on current remote work policies",
                Status = FormStatus.Draft,
                CreatedDate = new DateTime(2025, 10, 11),
                ResponseCount = 0,
                TotalRecipients = 48
            }
        };
    }

    #region Form Actions

    [RelayCommand]
    private async Task CreateSurvey()
    {
        await _navigationService.NavigateTo(ViewModelType.CreateSurvey);
    }

    [RelayCommand]
    private async Task ViewResponses(FormItemViewModel? form)
    {
        if (form == null) return;
        
        // TODO: Navigate to responses view
        // For now, just log the action
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task ViewResults(FormItemViewModel? form)
    {
        if (form == null) return;
        
        // TODO: Navigate to results view
        await Task.CompletedTask;
    }

    [RelayCommand]
    private void SendToEmployees(FormItemViewModel? form)
    {
        if (form == null) return;

        // Update the form status from Draft to Active
        form.Status = FormStatus.Active;
        form.TotalRecipients = 48; // Mock: sending to all employees
        form.ResponseCount = 0;
        
        // Force UI update by re-triggering property changes
        var index = Forms.IndexOf(form);
        if (index >= 0)
        {
            Forms.RemoveAt(index);
            Forms.Insert(index, form);
        }
    }

    #endregion

    public override async Task OnNavigatedToAsync()
    {
        Sidebar.CurrentPage = "Forms";
        // TODO: Load actual forms data from API
        await base.OnNavigatedToAsync();
    }
}

/// <summary>
/// Represents the status of a form/survey
/// </summary>
public enum FormStatus
{
    Draft,
    Active,
    Completed
}

/// <summary>
/// Represents a form/survey item in the list
/// </summary>
public partial class FormItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private FormStatus _status;

    [ObservableProperty]
    private DateTime _createdDate;

    [ObservableProperty]
    private int _responseCount;

    [ObservableProperty]
    private int _totalRecipients;

    public string CreatedDateDisplay => $"Created {CreatedDate:M/d/yyyy}";
    
    public string ResponseDisplay => $"{ResponseCount} / {TotalRecipients} responses";
    
    public double ResponsePercentage => TotalRecipients > 0 ? (double)ResponseCount / TotalRecipients * 100 : 0;
    
    public string ResponsePercentageDisplay => $"{ResponsePercentage:F0}%";

    public bool IsDraft => Status == FormStatus.Draft;
    public bool IsActive => Status == FormStatus.Active;
    public bool IsCompleted => Status == FormStatus.Completed;
    
    public bool ShowProgressBar => Status == FormStatus.Active;
    public bool ShowResponseCount => Status != FormStatus.Draft;

    public string StatusText => Status switch
    {
        FormStatus.Draft => "Draft",
        FormStatus.Active => "Active",
        FormStatus.Completed => "Completed",
        _ => "Unknown"
    };
}
