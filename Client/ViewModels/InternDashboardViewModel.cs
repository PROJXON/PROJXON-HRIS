using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Client.Models;
using Client.Services;
using Client.Utils.Classes;
using Client.Utils.Enums;
using Client.Utils.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Attendance;
using Shared.EmployeeManagement.Responses;

namespace Client.ViewModels;

public partial class InternDashboardViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly ISessionService _sessionService;
    private readonly IApiClient _apiClient;

    public SidebarViewModel Sidebar { get; }

    [ObservableProperty] private string _userName = string.Empty;
    [ObservableProperty] private double _hoursThisWeek = 0.0; 
    [ObservableProperty] private string _daysPresent = "0/5"; 
    [ObservableProperty] private int _pendingTasks;
    [ObservableProperty] private int _completedTasks;

    [ObservableProperty] private ObservableCollection<TaskItemResponse> _upcomingTasks = new();
    [ObservableProperty] private ObservableCollection<ActivityItem> _recentActivities = new();

    public InternDashboardViewModel(
        INavigationService navigationService,
        ISessionService sessionService,
        IApiClient apiClient,
        SidebarViewModel sidebar)
    {
        _navigationService = navigationService;
        _sessionService = sessionService;
        _apiClient = apiClient;
        Sidebar = sidebar;
    }

    public InternDashboardViewModel() : this(null!, null!, null!, new SidebarViewModel()) { }

    public override async Task OnNavigatedToAsync()
    {
        Sidebar.CurrentPage = "Dashboard";
        Sidebar.SetPortalMode(true);

        if (_sessionService.CurrentEmployee?.BasicInfo != null)
        {
            var basic = _sessionService.CurrentEmployee.BasicInfo;
            UserName = basic.PreferredName ?? basic.FirstName ?? "Intern";
        }

        await LoadDashboardData();
        await base.OnNavigatedToAsync();
    }

    private async Task LoadDashboardData()
    {
        var employeeId = _sessionService.CurrentEmployee?.Id ?? 0;
        if (employeeId == 0) return;

        var taskResult = await _apiClient.GetAllAsync<IEnumerable<TaskItemResponse>>($"api/Survey/tasks/{employeeId}");
        if (taskResult.IsSuccess && taskResult.Data != null)
        {
            var allTasks = taskResult.Data.ToList();

            PendingTasks = allTasks.Count(t => !t.IsCompleted);
            CompletedTasks = allTasks.Count(t => t.IsCompleted);

            UpcomingTasks = new ObservableCollection<TaskItemResponse>(
                allTasks.Where(t => !t.IsCompleted).OrderBy(t => t.DueDate).Take(2)
            );

            BuildRecentActivityFromTasks(allTasks.Where(t => t.IsCompleted));
        }

        var attResult = await _apiClient.GetAllAsync<IEnumerable<AttendanceResponse>>($"api/Attendance/{employeeId}");
        if (attResult.IsSuccess && attResult.Data != null)
        {
            var attendance = attResult.Data.ToList();
            
            // Current Week Logic
            var now = DateTime.UtcNow;
            var startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek); 
            var endOfWeek = startOfWeek.AddDays(7);
            var weekRecords = attendance.Where(a => a.Date >= startOfWeek && a.Date < endOfWeek).ToList();
            
            // Sum Hours
            HoursThisWeek = weekRecords.Sum(a => a.HoursWorked);
            
            // Days Present (Unique days worked this week / 5)
            int daysWorked = weekRecords.Select(a => a.Date.Date).Distinct().Count();
            DaysPresent = $"{daysWorked}/5";
            
            // Add Recent Attendance Activity
            var recent = attendance.OrderByDescending(a => a.Date).FirstOrDefault();
            if (recent != null)
            {
                RecentActivities.Insert(0, new ActivityItem 
                { 
                    Title = "Submitted attendance", 
                    Subtitle = $"{recent.HoursWorked:F1} hours on {recent.Date:MMM dd}", 
                    TimeAgo = GetTimeAgo(recent.Date)
                });
            }
        }
        else 
        {
            // Default if no attendance data
            HoursThisWeek = 0;
            DaysPresent = "0/5";
        }
    }

    private void BuildRecentActivityFromTasks(IEnumerable<TaskItemResponse> completedTasks)
    {
        RecentActivities.Clear();
        var activities = new List<(DateTime Date, ActivityItem Item)>();

        foreach (var task in completedTasks)
        {
            if (task.CompletedDate.HasValue)
            {
                activities.Add((task.CompletedDate.Value, new ActivityItem
                {
                    Title = $"Completed \"{task.Title}\"",
                    Subtitle = "Survey",
                    TimeAgo = GetTimeAgo(task.CompletedDate)
                }));
            }
        }

        var emp = _sessionService.CurrentEmployee;
        if (emp != null && emp.UpdateDateTime.HasValue && emp.CreateDateTime.HasValue)
        {
            if ((emp.UpdateDateTime.Value - emp.CreateDateTime.Value).TotalMinutes > 5)
            {
                activities.Add((emp.UpdateDateTime.Value, new ActivityItem
                {
                    Title = "Profile Updated",
                    Subtitle = "Personal Info",
                    TimeAgo = GetTimeAgo(emp.UpdateDateTime)
                }));
            }
        }

        foreach(var act in activities.OrderByDescending(x => x.Date).Take(2)) 
        {
            RecentActivities.Add(act.Item);
        }
    }

    private string GetTimeAgo(DateTime? date)
    {
        if (!date.HasValue) return "Recently";
        var span = DateTime.UtcNow - date.Value;
        
        if (span.TotalMinutes < 1) return "Just now";
        if (span.TotalMinutes < 60) return $"{span.TotalMinutes:F0} mins ago";
        if (span.TotalHours < 24) return $"{span.TotalHours:F0} hours ago";
        return $"{span.TotalDays:F0} days ago";
    }

    [RelayCommand]
    private async Task NavigateToTasks()
    {
        await _navigationService.NavigateTo(ViewModelType.Tasks);
    }
}