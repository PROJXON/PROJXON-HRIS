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

    [ObservableProperty]
    private bool _isPopupOpen;

    [ObservableProperty]
    private TimeOffRequestItemViewModel? _selectedRequest;

    [ObservableProperty]
    private string _popupActionText = string.Empty;

    [ObservableProperty]
    private string _popupActionButtonClass = string.Empty;

    public bool IsApprovingAction => PopupActionText == "Approve";
    public bool IsDecliningAction => PopupActionText == "Decline";

    public string PopupDescriptionText =>
        SelectedRequest == null ? "" :
        PopupActionText == "Approve" 
            ? $"Are you sure you want to approve the time off request from {SelectedRequest.EmployeeName} for {SelectedRequest.TotalDaysOff} day(s)?"
            : $"Are you sure you want to decline the time off request from {SelectedRequest.EmployeeName} for {SelectedRequest.TotalDaysOff} day(s)?";

    public string SelectedRequestDetails =>
    SelectedRequest == null ? "" :
    $"{SelectedRequest.EmployeeName} is requesting {SelectedRequest.TotalDaysOff} days off\n" +
    $"{SelectedRequest.LeaveFrom:MM/dd/yyyy} → {SelectedRequest.LeaveTo:MM/dd/yyyy}\n\n" +
    $"Reason: {SelectedRequest.Reason}";

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
                Reason = "Medical appointment"
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
                Reason = "Personal matters"
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
                Reason = "Sick leave"
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
    }

    [RelayCommand]
    private void DenyRequest(TimeOffRequestItemViewModel? request)
    {
        if (request == null || request.Status != RequestStatus.Pending) return;

        // Update the request status from Pending to Declined
        request.Status = RequestStatus.Declined;
    }

    [RelayCommand]
    private void OpenApprovePopup(TimeOffRequestItemViewModel request)
    {
        SelectedRequest = request;
        PopupActionText = "Approve";
        PopupActionButtonClass = "ApproveButton";
        OnPropertyChanged(nameof(IsApprovingAction));
        OnPropertyChanged(nameof(IsDecliningAction));
        OnPropertyChanged(nameof(PopupDescriptionText));
        IsPopupOpen = true;
    }

    [RelayCommand]
    private void OpenDeclinePopup(TimeOffRequestItemViewModel request)
    {
        SelectedRequest = request;
        PopupActionText = "Decline";
        PopupActionButtonClass = "DeclineButton";
        OnPropertyChanged(nameof(IsApprovingAction));
        OnPropertyChanged(nameof(IsDecliningAction));
        OnPropertyChanged(nameof(PopupDescriptionText));
        IsPopupOpen = true;
    }

    [RelayCommand]
    private void ClosePopup()
    {
        IsPopupOpen = false;
        SelectedRequest = null; // Clear selection when closing
    }

    [RelayCommand]
    private void ConfirmAction()
    {
        if (SelectedRequest == null)
            return;

        if (PopupActionText == "Approve")
        {
            SelectedRequest.Status = RequestStatus.Approved;
        }
        else if (PopupActionText == "Decline")
        {
            SelectedRequest.Status = RequestStatus.Declined;
        }

        // No need to manually update the collection - property notifications handle it
        IsPopupOpen = false;
        SelectedRequest = null; // Clear selection after action
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

    // This method is called automatically when Status changes (because of [ObservableProperty])
    partial void OnStatusChanged(RequestStatus value)
    {
        // Notify that all status-dependent properties have changed
        OnPropertyChanged(nameof(StatusText));
        OnPropertyChanged(nameof(IsPending));
        OnPropertyChanged(nameof(IsApproved));
        OnPropertyChanged(nameof(IsDeclined));
    }
}