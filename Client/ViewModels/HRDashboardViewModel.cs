using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Client.Models;
using Client.Models.EmployeeManagement;
using Client.Services;
using Client.Utils.Classes;
using Client.Utils.Enums;
using Client.Utils.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Attendance;
using Shared.CandidateManagement.Responses;
using Shared.EmployeeManagement.Responses;

namespace Client.ViewModels;

/// <summary>
/// ViewModel for the HR Portal Dashboard
/// Provides access to HR-specific features like employee management, recruitment, time off approval, etc.
/// </summary>
public partial class HRDashboardViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ISessionService _sessionService;
    private readonly IApiClient _apiClient;

    public SidebarViewModel Sidebar { get; }

    [ObservableProperty] private int _totalEmployees;
    [ObservableProperty] private int _pendingTimeOff = 0;
    [ObservableProperty] private int _activeRecruitments = 0;
    [ObservableProperty] private int _attendancePercentage = 0;
    [ObservableProperty] private ObservableCollection<ActivityItem> _recentActivities = new();

    public bool IsDevUser => ((MainWindowViewModel)Application.Current!.DataContext!).IsDevUser;
    public bool ShowDevOptions => IsDevUser && IsDevModeEnabled;

    public bool IsDevModeEnabled
    {
        get => ((MainWindowViewModel)Application.Current!.DataContext!).IsDevModeEnabled;
        set
        {
            ((MainWindowViewModel)Application.Current!.DataContext!).IsDevModeEnabled = value;
            OnPropertyChanged(nameof(ShowDevOptions));
            OnPropertyChanged(nameof(IsDevModeEnabled));
        }
    }

    public HRDashboardViewModel(
        INavigationService navigationService,
        IUserPreferencesService userPreferencesService,
        IEmployeeRepository employeeRepository,
        ISessionService sessionService,
        IApiClient apiClient,
        SidebarViewModel sidebarViewModel)
    {
        _navigationService = navigationService;
        _userPreferencesService = userPreferencesService;
        _employeeRepository = employeeRepository;
        _sessionService = sessionService;
        _apiClient = apiClient; 
        Sidebar = sidebarViewModel;
    }

    public HRDashboardViewModel() : this(null!, null!, null!, null!, null!, new SidebarViewModel()) { }

    public override async Task OnNavigatedToAsync()
    {
        Sidebar.CurrentPage = "Dashboard";
        Sidebar.SetPortalMode(false);
        await LoadDashboardDataAsync();
        await base.OnNavigatedToAsync();
    }

    public override async Task OnNavigatedFromAsync()
    {
        await Sidebar.OnNavigatedFromAsync();
        await base.OnNavigatedFromAsync();
    }

    private async Task LoadDashboardDataAsync()
    {
        var employeeResult = await _employeeRepository.GetAllAsync<EmployeeResponse>();
        var activities = new List<(DateTime Date, string Title, string Subtitle)>();

        if (employeeResult.IsSuccess && employeeResult.Value != null)
        {
            var employees = employeeResult.Value.ToList();
            TotalEmployees = employees.Count;
            
            // Employee Activities
            foreach (var emp in employees)
            {
                var name = $"{emp.BasicInfo?.FirstName} {emp.BasicInfo?.LastName}".Trim();
                
                if (emp.CreateDateTime.HasValue)
                    activities.Add((emp.CreateDateTime.Value, "New Employee Hired", $"{name} joined the team"));

                if (emp.UpdateDateTime.HasValue && emp.CreateDateTime.HasValue && 
                    (emp.UpdateDateTime.Value - emp.CreateDateTime.Value).TotalMinutes > 5)
                    activities.Add((emp.UpdateDateTime.Value, "Profile Updated", $"{name}'s profile updated"));
            }
        }

        // Attendance Percentage Calculation
        var attResult = await _apiClient.GetAllAsync<IEnumerable<AttendanceResponse>>("api/Attendance/all");
        
        if (attResult.IsSuccess && attResult.Data != null && TotalEmployees > 0)
        {
            var records = attResult.Data.ToList();
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            
            // Calculate Business Days so far in month (Mon-Fri)
            int businessDays = 0;
            for (var d = startOfMonth; d <= now.Date; d = d.AddDays(1))
            {
                if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                    businessDays++;
            }
            
            // Expected Total Record Count = Employees * BusinessDays
            int expected = TotalEmployees * businessDays;
            
            // Actual Records this month
            int actual = records.Count(r => r.Date >= startOfMonth && r.Date <= now);
            
            AttendancePercentage = expected > 0 
                ? (int)((double)actual / expected * 100) 
                : 0;
                
            if (AttendancePercentage > 100) AttendancePercentage = 100;
        }
        else 
        {
            AttendancePercentage = 0;
        }

        var surveysResult = await _apiClient.GetAllAsync<IEnumerable<SurveyResponse>>("api/Survey");
        if (surveysResult.IsSuccess && surveysResult.Data != null)
        {
            foreach(var s in surveysResult.Data.Where(x => x.CreatedDate > DateTime.UtcNow.AddDays(-30)))
            {
                activities.Add((s.CreatedDate, "Survey Created", $"\"{s.Title}\" was created"));
            }
        }

        // Fetch Survey Assignments to show completions
        try 
        {
            var assignmentsResult = await _apiClient.GetAllAsync<IEnumerable<SurveyAssignmentResponse>>("api/Survey/assignments");
            if (assignmentsResult.IsSuccess && assignmentsResult.Data != null)
            {
                foreach(var a in assignmentsResult.Data.Where(x => x.IsCompleted && x.CompletedDate.HasValue))
                {
                    activities.Add((a.CompletedDate!.Value, "Survey Completed", $"{a.EmployeeName} completed \"{a.Survey?.Title}\""));
                }
            }
        }
        catch { /* Ignore if endpoint fails during dev */ }

        RecentActivities.Clear();
        var recentItems = activities.OrderByDescending(x => x.Date).Take(4); // LIMITED TO 4
        
        foreach (var item in recentItems)
        {
            RecentActivities.Add(new ActivityItem
            {
                Title = item.Title,
                Subtitle = item.Subtitle,
                TimeAgo = GetTimeAgo(item.Date)
            });
        }

        try
        {
            var candidateResult = await _apiClient.GetAllAsync<IEnumerable<CandidateResponse>>("api/candidate");
            if (candidateResult.IsSuccess && candidateResult.Data != null)
            {
                ActiveRecruitments = candidateResult.Data.Count(c => 
                    !string.Equals(c.Status, "Hired", StringComparison.OrdinalIgnoreCase) && 
                    !string.Equals(c.Status, "Rejected", StringComparison.OrdinalIgnoreCase));
            }
        }
        catch { ActiveRecruitments = 0; }
    }

    private string GetTimeAgo(DateTime date)
    {
        var span = DateTime.UtcNow - date;
        if (span.TotalMinutes < 60) return $"{span.TotalMinutes:F0}m ago";
        if (span.TotalHours < 24) return $"{span.TotalHours:F0}h ago";
        return $"{span.TotalDays:F0}d ago";
    }

    [RelayCommand]
    private async Task SwitchToInternPortal()
    {
        await _userPreferencesService.ClearPortalPreferenceAsync();
        await _navigationService.NavigateTo(ViewModelType.InternDashboard);
    }
}