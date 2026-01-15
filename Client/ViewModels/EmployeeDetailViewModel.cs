using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Client.Models.EmployeeManagement;
using Client.Services;
using Client.Utils.Enums;
using Client.Utils.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Client.ViewModels;

/// <summary>
/// ViewModel for the Employee Detail view
/// Displays detailed employee information with tabs for Personal Info, Attendance, Documents, and Performance
/// </summary>
public partial class EmployeeDetailViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IEmployeeRepository? _employeeRepository;
    private int _currentEmployeeId;

    #region Sidebar User Profile

    [ObservableProperty]
    private string _sidebarUserName = "John Smith";

    [ObservableProperty]
    private string _sidebarUserRole = "HR Manager";

    #endregion

    #region Tab Selection

    [ObservableProperty]
    private bool _isPersonalInfoTabSelected = true;

    [ObservableProperty]
    private bool _isAttendanceTabSelected;

    #endregion

    #region Employee Information

    [ObservableProperty]
    private string _employeeFullName = string.Empty;

    [ObservableProperty]
    private string _employeeInitial = string.Empty;

    [ObservableProperty]
    private string _employeeJobTitle = string.Empty;

    [ObservableProperty]
    private string _employeeEmail = string.Empty;

    [ObservableProperty]
    private string _employeeDiscord = string.Empty;

    [ObservableProperty]
    private string _employeeDepartment = string.Empty;

    [ObservableProperty]
    private string _employeeId = string.Empty;

    [ObservableProperty]
    private string _employeePhone = string.Empty;

    [ObservableProperty]
    private string _employeeStartDate = string.Empty;

    [ObservableProperty]
    private string _employeeLocation = string.Empty;

    [ObservableProperty]
    private string _employeeType = string.Empty;

    #endregion

    #region Attendance

    [ObservableProperty]
    private ObservableCollection<AttendanceRecordViewModel> _attendanceRecords = new();

    [ObservableProperty]
    private bool _hasNoAttendanceRecords;

    #endregion

    #region Loading State

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    #endregion

    public EmployeeDetailViewModel(
        INavigationService navigationService,
        IEmployeeRepository? employeeRepository = null)
    {
        _navigationService = navigationService;
        _employeeRepository = employeeRepository;
    }

    // Parameterless constructor for design-time support
    public EmployeeDetailViewModel() : this(null!, null)
    {
        LoadMockData();
    }

    public void SetEmployeeId(int employeeId)
    {
        _currentEmployeeId = employeeId;
    }

    private void LoadMockData()
    {
        // Mock employee data matching the Figma design
        EmployeeFullName = "Alice Brown";
        EmployeeInitial = "A";
        EmployeeJobTitle = "Senior Developer";
        EmployeeEmail = "alice.brown@company.com";
        EmployeeDiscord = "alice_b#1234";
        EmployeeDepartment = "Engineering";
        EmployeeId = "EMP-0001";
        EmployeePhone = "(555) 123-4567";
        EmployeeStartDate = "January 15, 2022";
        EmployeeLocation = "San Francisco, CA";
        EmployeeType = "Full-time";

        // Mock attendance records
        AttendanceRecords = new ObservableCollection<AttendanceRecordViewModel>
        {
            new()
            {
                Date = new DateTime(2025, 10, 13),
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(17, 30, 0)
            },
            new()
            {
                Date = new DateTime(2025, 10, 12),
                StartTime = new TimeSpan(9, 15, 0),
                EndTime = new TimeSpan(17, 45, 0)
            },
            new()
            {
                Date = new DateTime(2025, 10, 9),
                StartTime = new TimeSpan(8, 45, 0),
                EndTime = new TimeSpan(17, 15, 0)
            },
            new()
            {
                Date = new DateTime(2025, 10, 8),
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(18, 0, 0)
            },
            new()
            {
                Date = new DateTime(2025, 10, 7),
                StartTime = new TimeSpan(8, 30, 0),
                EndTime = new TimeSpan(17, 0, 0)
            }
        };

        HasNoAttendanceRecords = AttendanceRecords.Count == 0;
    }

    #region Tab Commands

    [RelayCommand]
    private void SelectPersonalInfoTab()
    {
        IsPersonalInfoTabSelected = true;
        IsAttendanceTabSelected = false;
    }

    [RelayCommand]
    private void SelectAttendanceTab()
    {
        IsPersonalInfoTabSelected = false;
        IsAttendanceTabSelected = true;
    }

    #endregion

    #region Navigation Commands

    [RelayCommand]
    private async Task GoBack()
    {
        await _navigationService.NavigateTo(ViewModelType.Employees);
    }

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
        await _navigationService.NavigateTo(ViewModelType.TimeOffRequests);
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
        if (_currentEmployeeId > 0 && _employeeRepository != null)
        {
            await LoadEmployeeAsync();
        }
        else
        {
            LoadMockData();
        }

        await base.OnNavigatedToAsync();
    }

    private async Task LoadEmployeeAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var result = await _employeeRepository!.GetByIdAsync(_currentEmployeeId);
            if (result.IsSuccess && result.Value != null)
            {
                var employee = result.Value;

                EmployeeFullName = $"{employee.BasicInfo?.FirstName} {employee.BasicInfo?.LastName}".Trim();
                EmployeeInitial = !string.IsNullOrEmpty(employee.BasicInfo?.FirstName)
                    ? employee.BasicInfo.FirstName[0].ToString().ToUpper()
                    : "?";
                EmployeeJobTitle = employee.PositionDetails?.PositionName ?? "Unknown Position";
                EmployeeEmail = employee.ContactInfo?.ProjxonEmail ?? employee.ContactInfo?.PersonalEmail ?? string.Empty;
                EmployeeDiscord = "user#0000"; // TODO: Add discord to DTO
                EmployeeDepartment = "Engineering"; // TODO: Map from department ID
                EmployeeId = $"EMP-{employee.Id:D4}";
                EmployeePhone = employee.ContactInfo?.PhoneNumber ?? string.Empty;
                EmployeeStartDate = employee.PositionDetails?.HireDate?.ToString("MMMM dd, yyyy") ?? "N/A";
                EmployeeLocation = GetLocationString(employee.ContactInfo?.PermanentAddress);
                EmployeeType = employee.PositionDetails?.EmploymentType?.ToString() ?? "N/A";

                // TODO: Load actual attendance records from API
                LoadMockAttendanceRecords();
            }
            else
            {
                ErrorMessage = result.ErrorMessage;
                LoadMockData();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            LoadMockData();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void LoadMockAttendanceRecords()
    {
        AttendanceRecords = new ObservableCollection<AttendanceRecordViewModel>
        {
            new()
            {
                Date = new DateTime(2025, 10, 13),
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(17, 30, 0)
            },
            new()
            {
                Date = new DateTime(2025, 10, 12),
                StartTime = new TimeSpan(9, 15, 0),
                EndTime = new TimeSpan(17, 45, 0)
            },
            new()
            {
                Date = new DateTime(2025, 10, 9),
                StartTime = new TimeSpan(8, 45, 0),
                EndTime = new TimeSpan(17, 15, 0)
            }
        };

        HasNoAttendanceRecords = AttendanceRecords.Count == 0;
    }

    private static string GetLocationString(Shared.EmployeeManagement.Responses.AddressResponse? address)
    {
        if (address == null) return "N/A";

        var parts = new[]
        {
            address.City,
            address.StateOrProvince
        };

        return string.Join(", ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
    }
}

/// <summary>
/// Represents an attendance record entry
/// </summary>
public partial class AttendanceRecordViewModel : ObservableObject
{
    [ObservableProperty]
    private DateTime _date;

    [ObservableProperty]
    private TimeSpan _startTime;

    [ObservableProperty]
    private TimeSpan _endTime;

    public string DateDisplay => Date.ToString("dddd, MMMM d, yyyy");

    public string TimeRange => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";

    public string TotalHours
    {
        get
        {
            var duration = EndTime - StartTime;
            return $"{duration.TotalHours:F1} hrs";
        }
    }
}
