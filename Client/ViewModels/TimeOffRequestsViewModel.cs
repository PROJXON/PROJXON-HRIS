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
/// ViewModel for Time Off Requests
/// Displays a list of Time Off Requests from Employees
/// </summary>
public partial class TimeOffRequestsViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    #region Sidebar User Profile

    [ObservableProperty]
    private string _userName = "John Smith";

    [ObservableProperty]
    private string _userRole = "HR Manager";

    #endregion

    #region Time Off Requests Data

    [ObservableProperty]
    private ObservableCollection<TimeOffRequestItemViewModel> _timeOffRequestItem = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    #endregion

    public TimeOffRequestsViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        LoadMockData();
    }

    // Parameterless constructor for design-time support
    public TimeOffRequestsViewModel() : this(null!)
    {
    }

    private void LoadMockData()
    {
        TimeOffRequestItem = new ObservableCollection<TimeOffRequestItemViewModel>
        {
            new()
            {
                Id = 1,
                EmployeeName = "Sarah Johnson",
                LeaveFrom = new DateTime(2025, 10, 19),
                LeaveTo = new DateTime(2025, 10, 21),
                TotalDaysOff = 3,
                LeaveType = LeaveType.PTO,
                Status = RequestStatus.Pending,
                Reason = "Family vacation"
            },
            new()
            {
                Id = 2,
                EmployeeName = "Mike Wilson",
                LeaveFrom = new DateTime(2025, 10, 24),
                LeaveTo = new DateTime(2025, 10, 24),
                TotalDaysOff = 1,
                LeaveType = LeaveType.SickLeave,
                Status = RequestStatus.Pending,
                Reason = "Family vacation"
            },
            new()
            {
                Id = 3,
                EmployeeName = "Emily Davis",
                LeaveFrom = new DateTime(2025, 10, 31),
                LeaveTo = new DateTime(2025, 11, 04),
                TotalDaysOff = 5,
                LeaveType = LeaveType.PersonalLeave,
                Status = RequestStatus.Pending,
                Reason = "Family vacation"
            },
            new()
            {
                Id = 4,
                EmployeeName = "John Smith",
                LeaveFrom = new DateTime(2025, 10, 14),
                LeaveTo = new DateTime(2025, 10, 16),
                TotalDaysOff = 3,
                LeaveType = LeaveType.PTO,
                Status = RequestStatus.Approved,
                Reason = "Family vacation"
            },
            new()
            {
                Id = 5,
                EmployeeName = "Lisa Anderson",
                LeaveFrom = new DateTime(2025, 10, 9),
                LeaveTo = new DateTime(2025, 10, 10),
                TotalDaysOff = 2,
                LeaveType = LeaveType.SickLeave,
                Status = RequestStatus.Declined,
                Reason = "Family vacation"
            }
        };
    }

    #region Request Actions

    [RelayCommand]
    private void ApproveRequest(TimeOffRequestItemViewModel? request)
    {
        if (request == null || request.Status != RequestStatus.Pending) return;

        // Update the request status from Pending to Approved
        request.Status = RequestStatus.Approved;
        
        // Force UI update by re-triggering property changes
        var index = TimeOffRequestItem.IndexOf(request);
        if (index >= 0)
        {
            TimeOffRequestItem.RemoveAt(index);
            TimeOffRequestItem.Insert(index, request);
        }
    }

    [RelayCommand]
    private void DenyRequest(TimeOffRequestItemViewModel? request)
    {
        if (request == null || request.Status != RequestStatus.Pending) return;

        // Update the request status from Pending to Declined
        request.Status = RequestStatus.Declined;
        
        // Force UI update by re-triggering property changes
        var index = TimeOffRequestItem.IndexOf(request);
        if (index >= 0)
        {
            TimeOffRequestItem.RemoveAt(index);
            TimeOffRequestItem.Insert(index, request);
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
        // Already on Time Off Requests page
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
        await _navigationService.NavigateTo(ViewModelType.Recruitment);
    }

    [RelayCommand]
    private async Task NavigateToForms()
    {
        await _navigationService.NavigateTo(ViewModelType.Forms);
    }

    #endregion

    public override async Task OnNavigatedToAsync()
    {
        // TODO: Load actual request data from API
        await base.OnNavigatedToAsync();
    }
}

/// <summary>
/// Represents the status of a Time Off Request
/// </summary>
public enum RequestStatus
{
    Pending,
    Approved,
    Declined
}

public enum LeaveType
{
    PTO,
    SickLeave,
    PersonalLeave,
    MedicalLeave
}

/// <summary>
/// Represents a Time Off Request item in the list
/// </summary>
public partial class TimeOffRequestItemViewModel : ObservableObject
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _employeeName = string.Empty;

    [ObservableProperty]
    private DateTime _leaveFrom;

    [ObservableProperty]
    private DateTime _leaveTo;

    [ObservableProperty]
    private int _totalDaysOff;

    [ObservableProperty]
    private LeaveType _leaveType;

    [ObservableProperty]
    private RequestStatus _status;

    [ObservableProperty]
    private string _reason = string.Empty;

    public string LeaveFromDateDisplay => $"Created {LeaveFrom:M/d/yyyy}";
    public string LeaveToDateDisplay => $"Created {LeaveTo:M/d/yyyy}";
    public string TotalDaysDisplay => TotalDaysOff == 1 ? "1 day" : $"{TotalDaysOff} days";

    public bool IsPending => Status == RequestStatus.Pending;
    public bool IsApproved => Status == RequestStatus.Approved;
    public bool IsDeclined => Status == RequestStatus.Declined;

    public string StatusText => Status switch
    {
        RequestStatus.Pending => "Pending",
        RequestStatus.Approved => "Approved",
        RequestStatus.Declined => "Declined",
        _ => "Unknown"
    };
}
