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
using Shared.Attendance;

namespace Client.ViewModels;

/// <summary>
/// ViewModel for the Attendance Calendar view
/// Displays a monthly calendar with attendance entries and allows time entry
/// </summary>
public partial class AttendanceViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly IApiClient _apiClient;
    private readonly ISessionService _sessionService;
    
    // Sidebar handled by MainWindow now, injected to set properties
    private readonly SidebarViewModel _sidebarViewModel;

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

    public AttendanceViewModel(
        INavigationService navigationService, 
        SidebarViewModel sidebarViewModel,
        IUserPreferencesService userPreferencesService,
        IApiClient apiClient,
        ISessionService sessionService)
    {
        _navigationService = navigationService;
        _sidebarViewModel = sidebarViewModel;
        _userPreferencesService = userPreferencesService;
        _apiClient = apiClient;
        _sessionService = sessionService;
        
        CurrentMonth = DateTime.UtcNow;
        GenerateCalendar();
    }

    // Parameterless constructor for design-time support
    public AttendanceViewModel() : this(null!, null!, null!, null!, null!)
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
        var today = DateTime.UtcNow.Date;
        
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

    private async Task LoadAttendanceData()
    {
        if (_sessionService.CurrentEmployee == null) return;
        
        try 
        {
            var result = await _apiClient.GetAllAsync<IEnumerable<AttendanceResponse>>($"api/Attendance/{_sessionService.CurrentEmployee.Id}");
            
            if (result.IsSuccess && result.Data != null)
            {
                var records = result.Data.ToList();
                
                // Re-generate calendar to clear any existing data
                GenerateCalendar();

                foreach (var day in CalendarDays)
                {
                    // Skip empty cells
                    if (day.IsEmpty) continue;
                    
                    // Match by Date only
                    var record = records.FirstOrDefault(r => r.Date.Date == day.Date.Date);
                    if (record != null)
                    {
                        day.HasAttendance = true;
                        day.StartTime = record.StartTime;
                        day.EndTime = record.EndTime;
                        day.TimeDisplay = $"{record.StartTime:hh\\:mm}-\n{record.EndTime:hh\\:mm}";
                    }
                }
            }
        }
        catch (Exception) 
        { 
            // TODO: Log error
        }
    }

    #region Calendar Navigation Commands

    [RelayCommand]
    private void PreviousMonth()
    {
        CurrentMonth = CurrentMonth.AddMonths(-1);
        GenerateCalendar();
        ClearTimeEntry();
        _ = LoadAttendanceData(); // Fire and forget
    }

    [RelayCommand]
    private void NextMonth()
    {
        CurrentMonth = CurrentMonth.AddMonths(1);
        GenerateCalendar();
        ClearTimeEntry();
        _ = LoadAttendanceData(); // Fire and forget
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
        if (SelectedDay == null || _sessionService.CurrentEmployee == null) 
            return;

        if (SelectedDay.StartTime.HasValue && SelectedDay.EndTime.HasValue)
        {
            var request = new CreateAttendanceRequest
            {
                EmployeeId = _sessionService.CurrentEmployee.Id ?? 0,
                Date = SelectedDay.Date,
                StartTime = SelectedDay.StartTime.Value,
                EndTime = SelectedDay.EndTime.Value
            };

            var result = await _apiClient.PostAsync<AttendanceResponse>("api/Attendance", request);

            if (result.IsSuccess)
            {
                SelectedDay.HasAttendance = true;
                SelectedDay.TimeDisplay = $"{SelectedDay.StartTime.Value:hh\\:mm}-\n{SelectedDay.EndTime.Value:hh\\:mm}";
                
                // Update local list view
                OnPropertyChanged(nameof(CalendarDays));
                
                IsTimePickerOpen = false;
            }
        }
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

    public override async Task OnNavigatedToAsync()
    {
        _sidebarViewModel.CurrentPage = "Attendance";
        
        // Determine portal mode from saved preference
        var portalPref = await _userPreferencesService.GetPortalPreferenceAsync();
        _sidebarViewModel.SetPortalMode(portalPref == PortalType.Intern);
        
        // Load actual attendance data from API
        await LoadAttendanceData();
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