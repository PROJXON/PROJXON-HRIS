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
/// ViewModel for the Attendance Calendar view
/// Displays a monthly calendar with attendance entries and allows time entry
/// </summary>
public partial class AttendanceViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    #region Sidebar User Profile

    [ObservableProperty]
    private string _userName = "John Smith";

    [ObservableProperty]
    private string _userRole = "HR Manager";

    #endregion

    #region Calendar State

    [ObservableProperty]
    private DateTime _currentMonth;

    [ObservableProperty]
    private string _monthYearDisplay = string.Empty;

    [ObservableProperty]
    private ObservableCollection<CalendarDayViewModel> _calendarDays = new();

    [ObservableProperty]
    private CalendarDayViewModel? _selectedDay;

    #endregion

    #region Time Entry Panel

    [ObservableProperty]
    private bool _isTimeEntryVisible;

    [ObservableProperty]
    private string _selectedDateDisplay = string.Empty;

    [ObservableProperty]
    private string _startTimeText = "--:-- --";

    [ObservableProperty]
    private string _endTimeText = "--:-- --";

    [ObservableProperty]
    private bool _isTimePickerOpen;

    [ObservableProperty]
    private bool _isStartTimePicker = true;

    [ObservableProperty]
    private int _selectedHour = 8;

    [ObservableProperty]
    private int _selectedMinute = 0;

    [ObservableProperty]
    private bool _isAm = true;

    [ObservableProperty]
    private ObservableCollection<int> _hours = new(Enumerable.Range(1, 12));

    [ObservableProperty]
    private ObservableCollection<int> _minutes = new(new[] { 0, 15, 30, 45 });

    #endregion

    public AttendanceViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        _currentMonth = new DateTime(2025, 10, 1); // October 2025 as shown in Figma
        GenerateCalendar();
    }

    // Parameterless constructor for design-time support
    public AttendanceViewModel() : this(null!)
    {
    }

    private void GenerateCalendar()
    {
        MonthYearDisplay = CurrentMonth.ToString("MMMM yyyy");
        CalendarDays.Clear();

        var firstDayOfMonth = new DateTime(CurrentMonth.Year, CurrentMonth.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
        var daysInMonth = lastDayOfMonth.Day;

        // Get the day of week for the first day (0 = Sunday)
        var startDayOfWeek = (int)firstDayOfMonth.DayOfWeek;

        // Add empty cells for days before the first of the month
        for (int i = 0; i < startDayOfWeek; i++)
        {
            CalendarDays.Add(new CalendarDayViewModel
            {
                IsEmpty = true,
                Date = DateTime.MinValue
            });
        }

        // Add actual days
        var today = new DateTime(2025, 10, 15); // Simulating "today" for the demo
        
        for (int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(CurrentMonth.Year, CurrentMonth.Month, day);
            var calendarDay = new CalendarDayViewModel
            {
                Date = date,
                DayNumber = day,
                IsToday = date == today,
                IsEmpty = false
            };

            // Add mock attendance data matching the Figma design
            SetMockAttendanceData(calendarDay, day);

            CalendarDays.Add(calendarDay);
        }

        // Add empty cells to complete the last week
        var totalCells = CalendarDays.Count;
        var remainingCells = (7 - (totalCells % 7)) % 7;
        for (int i = 0; i < remainingCells; i++)
        {
            CalendarDays.Add(new CalendarDayViewModel
            {
                IsEmpty = true,
                Date = DateTime.MinValue
            });
        }
    }

    private void SetMockAttendanceData(CalendarDayViewModel day, int dayNumber)
    {
        // Based on the Figma design
        switch (dayNumber)
        {
            case 10:
                day.HasAttendance = true;
                day.StartTime = new TimeSpan(8, 45, 0);
                day.EndTime = new TimeSpan(17, 15, 0);
                day.TimeDisplay = "08:45-\n17:15";
                break;
            case 13:
                day.HasAttendance = true;
                day.StartTime = new TimeSpan(9, 15, 0);
                day.EndTime = new TimeSpan(17, 45, 0);
                day.TimeDisplay = "09:15-\n17:45";
                break;
            case 14:
                day.HasAttendance = true;
                day.StartTime = new TimeSpan(9, 0, 0);
                day.EndTime = new TimeSpan(17, 30, 0);
                day.TimeDisplay = "09:00-\n17:30";
                break;
            case 20:
            case 21:
            case 22:
                day.IsOnLeave = true;
                day.LeaveText = "Leave";
                break;
        }
    }

    #region Calendar Navigation Commands

    [RelayCommand]
    private void PreviousMonth()
    {
        CurrentMonth = CurrentMonth.AddMonths(-1);
        GenerateCalendar();
        ClearTimeEntry();
    }

    [RelayCommand]
    private void NextMonth()
    {
        CurrentMonth = CurrentMonth.AddMonths(1);
        GenerateCalendar();
        ClearTimeEntry();
    }

    #endregion

    #region Day Selection Commands

    [RelayCommand]
    private void SelectDay(CalendarDayViewModel? day)
    {
        if (day == null || day.IsEmpty || day.IsOnLeave)
            return;

        // Deselect previous
        if (SelectedDay != null)
        {
            SelectedDay.IsSelected = false;
        }

        // Select new
        day.IsSelected = true;
        SelectedDay = day;

        // Show time entry panel
        IsTimeEntryVisible = true;
        SelectedDateDisplay = day.Date.ToString("M/d/yyyy");

        // Populate existing time if available
        if (day.HasAttendance && day.StartTime.HasValue && day.EndTime.HasValue)
        {
            StartTimeText = FormatTimeSpan(day.StartTime.Value);
            EndTimeText = FormatTimeSpan(day.EndTime.Value);
        }
        else
        {
            StartTimeText = "--:-- --";
            EndTimeText = "--:-- --";
        }

        IsTimePickerOpen = false;
    }

    private static string FormatTimeSpan(TimeSpan time)
    {
        var hour = time.Hours;
        var minute = time.Minutes;
        var amPm = hour >= 12 ? "PM" : "AM";
        
        if (hour > 12) hour -= 12;
        if (hour == 0) hour = 12;
        
        return $"{hour:D2}:{minute:D2} {amPm}";
    }

    #endregion

    #region Time Picker Commands

    [RelayCommand]
    private void OpenStartTimePicker()
    {
        IsStartTimePicker = true;
        IsTimePickerOpen = true;
        
        // Parse current start time if set
        if (SelectedDay?.StartTime.HasValue == true)
        {
            var time = SelectedDay.StartTime.Value;
            SelectedHour = time.Hours > 12 ? time.Hours - 12 : (time.Hours == 0 ? 12 : time.Hours);
            SelectedMinute = time.Minutes;
            IsAm = time.Hours < 12;
        }
        else
        {
            SelectedHour = 8;
            SelectedMinute = 45;
            IsAm = true;
        }
    }

    [RelayCommand]
    private void OpenEndTimePicker()
    {
        IsStartTimePicker = false;
        IsTimePickerOpen = true;
        
        // Parse current end time if set
        if (SelectedDay?.EndTime.HasValue == true)
        {
            var time = SelectedDay.EndTime.Value;
            SelectedHour = time.Hours > 12 ? time.Hours - 12 : (time.Hours == 0 ? 12 : time.Hours);
            SelectedMinute = time.Minutes;
            IsAm = time.Hours < 12;
        }
        else
        {
            SelectedHour = 5;
            SelectedMinute = 0;
            IsAm = false;
        }
    }

    [RelayCommand]
    private void SelectHour(int hour)
    {
        SelectedHour = hour;
        UpdateTimeFromPicker();
    }

    [RelayCommand]
    private void SelectMinute(int minute)
    {
        SelectedMinute = minute;
        UpdateTimeFromPicker();
    }

    [RelayCommand]
    private void SetAm()
    {
        IsAm = true;
        UpdateTimeFromPicker();
    }

    [RelayCommand]
    private void SetPm()
    {
        IsAm = false;
        UpdateTimeFromPicker();
    }

    [RelayCommand]
    private void CloseTimePicker()
    {
        IsTimePickerOpen = false;
    }

    private void UpdateTimeFromPicker()
    {
        var hour = SelectedHour;
        if (!IsAm && hour != 12) hour += 12;
        if (IsAm && hour == 12) hour = 0;

        var time = new TimeSpan(hour, SelectedMinute, 0);
        var timeText = FormatTimeSpan(time);

        if (IsStartTimePicker)
        {
            StartTimeText = timeText;
            if (SelectedDay != null)
            {
                SelectedDay.StartTime = time;
            }
        }
        else
        {
            EndTimeText = timeText;
            if (SelectedDay != null)
            {
                SelectedDay.EndTime = time;
            }
        }
    }

    #endregion

    #region Save/Cancel Commands

    [RelayCommand]
    private async Task SaveTimeEntry()
    {
        if (SelectedDay == null)
            return;

        // Update the calendar day with the new times
        if (SelectedDay.StartTime.HasValue && SelectedDay.EndTime.HasValue)
        {
            SelectedDay.HasAttendance = true;
            SelectedDay.TimeDisplay = $"{SelectedDay.StartTime.Value.Hours:D2}:{SelectedDay.StartTime.Value.Minutes:D2}-\n{SelectedDay.EndTime.Value.Hours:D2}:{SelectedDay.EndTime.Value.Minutes:D2}";
        }

        // TODO: Call API to save attendance
        await Task.Delay(100); // Simulate save

        IsTimePickerOpen = false;
    }

    [RelayCommand]
    private void CancelTimeEntry()
    {
        ClearTimeEntry();
    }

    private void ClearTimeEntry()
    {
        if (SelectedDay != null)
        {
            SelectedDay.IsSelected = false;
            SelectedDay = null;
        }

        IsTimeEntryVisible = false;
        IsTimePickerOpen = false;
        StartTimeText = "--:-- --";
        EndTimeText = "--:-- --";
        SelectedDateDisplay = string.Empty;
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
        // Already on attendance page
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task NavigateToEmployees()
    {
        await _navigationService.NavigateTo(ViewModelType.Employees);
    }

    [RelayCommand]
    private async Task SwitchToInternPortal()
    {
        await _navigationService.NavigateTo(ViewModelType.InternDashboard);
    }

    #endregion

    public override async Task OnNavigatedToAsync()
    {
        // TODO: Load actual attendance data from API
        await base.OnNavigatedToAsync();
    }
}

/// <summary>
/// Represents a single day in the calendar
/// </summary>
public partial class CalendarDayViewModel : ObservableObject
{
    [ObservableProperty]
    private DateTime _date;

    [ObservableProperty]
    private int _dayNumber;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isToday;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private bool _hasAttendance;

    [ObservableProperty]
    private bool _isOnLeave;

    [ObservableProperty]
    private string _timeDisplay = string.Empty;

    [ObservableProperty]
    private string _leaveText = string.Empty;

    [ObservableProperty]
    private TimeSpan? _startTime;

    [ObservableProperty]
    private TimeSpan? _endTime;

    [ObservableProperty]
    private bool _isHovered;
}
