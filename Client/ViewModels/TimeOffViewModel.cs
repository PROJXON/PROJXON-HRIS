using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.Utils.Enums;

namespace Client.ViewModels;
using Client.Models;

/// <summary>
/// ViewModel for the Time Off page
/// </summary>
public partial class TimeOffViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    #region User Profile Properties

    [ObservableProperty]
    private string _userName = "Intern's Name";

    [ObservableProperty]
    private string _userRole = "Intern";

    #endregion

    #region Time Off Statistics

    [ObservableProperty]
    private int _availableTimeOff = 12;

    [ObservableProperty]
    private int _daysOffThisYear = 8;

    [ObservableProperty]
    private int _pendingRequests = 1;

    #endregion

    #region Time Off Requests

    [ObservableProperty]
    private ObservableCollection<TimeOffRequest> _timeOffRequests = new();

    #endregion

    #region Selected Menu Item

    [ObservableProperty]
    private string _selectedMenuItem = "Time Off";

    #endregion

    #region New Time Off Request Dialog
    [ObservableProperty]
    private bool _isNewRequestDialogOpen;

    [ObservableProperty]
    private string _newRequestType = "PTO";

    [ObservableProperty]
    private string _newRequestDateRange = string.Empty;

    [ObservableProperty]
    private string _newRequestReason = string.Empty;
    #endregion


    public TimeOffViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        LoadTimeOffRequests();
    }

    private void LoadTimeOffRequests()
    {
        TimeOffRequests = new ObservableCollection<TimeOffRequest>
        {
            new TimeOffRequest
            {
                Title = "PTO",
                Status = "Pending",
                TimeOffDate = "10/19/2025 - 10/21/2025",
                TotalDays = "3",
                Reason = "Family Vacation",
            },

            new TimeOffRequest
            {
                Title = "Sick Leave",
                Status = "Approved",
                TimeOffDate = "9/14/2025 - 9/15/2025",
                TotalDays = "1",
                Reason = "Not Feeling Well",
            },
        };
    }

    public override async Task OnNavigatedToAsync()
    {
        // TODO: Load real data from services
        // await LoadDashboardDataAsync();
        
        await base.OnNavigatedToAsync();
    }

    #region New Request Commands
    [RelayCommand]
    private void OpenNewRequestDialog()
    {
        NewRequestType = "PTO";
        NewRequestDateRange = string.Empty;
        NewRequestReason = string.Empty;

        IsNewRequestDialogOpen = true;
    }

    [RelayCommand]
    private void CloseNewRequestDialog()
    {
        IsNewRequestDialogOpen = false;
    }

    [RelayCommand]
    private void SubmitNewRequest()
    {
        if (string.IsNullOrWhiteSpace(NewRequestDateRange))
            return;

        var newRequest = new TimeOffRequest
        {
            Title = NewRequestType,
            TimeOffDate = NewRequestDateRange,
            Reason = NewRequestReason,
            Status = "Pending",
            TotalDays = "1"
        };

        TimeOffRequests.Add(newRequest);
        PendingRequests++;

        IsNewRequestDialogOpen = false;
    }
    #endregion

    #region Navigation Commands

    [RelayCommand]
    private async Task NavigateToDashboard()
    {
        SelectedMenuItem = "Dashboard";
        await _navigationService.NavigateTo(ViewModelType.InternDashboard);
    }

    [RelayCommand]
    private async Task NavigateToProfile()
    {
        SelectedMenuItem = "My Profile";
        // TODO: Navigate to profile view when implemented
        // await _navigationService.NavigateTo(ViewModelType.Profile);
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task NavigateToAttendance()
    {
        SelectedMenuItem = "My Attendance";
        // TODO: Navigate to attendance view when implemented
        // await _navigationService.NavigateTo(ViewModelType.TimeOffRequests);
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task NavigateToTimeOff()
    {
        SelectedMenuItem = "Time Off";
        // Already on Time Off page, could refresh data
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task NavigateToTasks()
    {
        SelectedMenuItem = "Tasks";
        await Task.CompletedTask;
    }

    #endregion
}

public class TimeOffRequest
{
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string TimeOffDate { get; set; } = string.Empty;
    public string TotalDays { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}