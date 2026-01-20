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
using Shared.EmployeeManagement.Responses;
using Client.Models.EmployeeManagement;

namespace Client.ViewModels;

/// <summary>
/// ViewModel for the Forms view
/// Displays a list of forms/surveys with their status and response statistics
/// </summary>
public partial class FormsViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IApiClient _apiClient;
    private readonly IEmployeeRepository _employeeRepository;

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

    public FormsViewModel(INavigationService navigationService, SidebarViewModel sidebarViewModel, IApiClient apiClient, IEmployeeRepository employeeRepository)
    {
        _navigationService = navigationService;
        Sidebar = sidebarViewModel;
        _apiClient = apiClient;
        _employeeRepository = employeeRepository;
    }

    // Parameterless constructor for design-time support
    public FormsViewModel() : this(null!, null!, null!, null!)
    {
    }

    private async Task LoadFormsAsync()
    {
        IsLoading = true;
        try
        {
            int totalEmployees = 0;
            var empResult = await _employeeRepository.GetAllAsync<EmployeeResponse>();
            if (empResult.IsSuccess && empResult.Value != null)
                totalEmployees = empResult.Value.Count();

            var assignmentResult = await _apiClient.GetAllAsync<IEnumerable<SurveyAssignmentResponse>>("api/Survey/assignments");
            var allAssignments = assignmentResult.IsSuccess && assignmentResult.Data != null 
                ? assignmentResult.Data.ToList() 
                : new List<SurveyAssignmentResponse>();

            var result = await _apiClient.GetAllAsync<IEnumerable<SurveyResponse>>("api/Survey");
            if (result.IsSuccess && result.Data != null)
            {
                Forms.Clear();
                foreach (var s in result.Data)
                {
                    // Calculate based on assignments
                    var specificAssignments = allAssignments.Where(a => a.SurveyId == s.Id).ToList();
                    
                    int activeRecipients = specificAssignments.Count;
                    int completedCount = specificAssignments.Count(a => a.IsCompleted);

                    // Fallback for UI if no assignments exist but survey is active
                    if (s.IsActive && activeRecipients == 0) activeRecipients = totalEmployees;

                    Forms.Add(new FormItemViewModel
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Description = s.Description,
                        Status = s.IsActive ? FormStatus.Active : FormStatus.Draft,
                        CreatedDate = s.CreatedDate,
                        ResponseCount = completedCount,
                        TotalRecipients = activeRecipients
                    });
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
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

        // Placeholder as no specific view exists yet, but ensures button isn't dead
        // Ideally navigate to a details page
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
    private async Task SendToEmployees(FormItemViewModel? form)
    {
        if (form == null) return;

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            // Call Backend to generate assignments for all employees
            var response = await _apiClient.PostAsync<object>($"api/Survey/{form.Id}/assign-all", null);

            if (response.IsSuccess)
            {
                // Reload to refresh counts/status
                await LoadFormsAsync();
            }
            else
            {
                ErrorMessage = "Failed to assign: " + response.ErrorMessage;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion

    public override async Task OnNavigatedToAsync()
    {
        Sidebar.CurrentPage = "Forms";
        Sidebar.SetPortalMode(false);
        await LoadFormsAsync();
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