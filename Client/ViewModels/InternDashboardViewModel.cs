using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.Utils.Enums;

namespace Client.ViewModels;
using Client.Models;

/// <summary>
/// ViewModel for the Intern Portal Dashboard
/// Provides access to intern-specific features like profile, attendance, tasks, and time off requests
/// </summary>
public partial class InternDashboardViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    #region User Profile Properties

    [ObservableProperty]
    private string _userName = "Intern's Name";

    [ObservableProperty]
    private string _userRole = "Intern";

    #endregion

    #region Dashboard Statistics

    [ObservableProperty]
    private int _hoursThisWeek = 32;

    [ObservableProperty]
    private int _daysPresent = 12;

    [ObservableProperty]
    private int _pendingTasks = 2;

    [ObservableProperty]
    private int _completedTasks = 5;

    #endregion

    #region Recent Activity

    [ObservableProperty]
    private ObservableCollection<TaskItem> _upcomingTasks = new();


    [ObservableProperty]
    private ObservableCollection<ActivityItem> _recentActivities = new();

    #endregion

    #region Selected Menu Item

    [ObservableProperty]
    private string _selectedMenuItem = "Dashboard";

    #endregion

    public InternDashboardViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        LoadUpcomingTasks();
        LoadRecentActivities();
    }

    private void LoadUpcomingTasks()
    {
        UpcomingTasks = new ObservableCollection<TaskItem>
        {
            new TaskItem { Title="Complete Employee Satisfaction Survey", DueDate="Due: October 18, 2025" },
            new TaskItem { Title="Submit Weekly Report", DueDate="Due: October 20, 2025" }
        };
    }

    private void LoadRecentActivities()
    {
        RecentActivities = new ObservableCollection<ActivityItem>
        {
            new ActivityItem
            {
                Title = "Submitted attendance",
                TimeAgo = "2 hours ago"
            },
            new ActivityItem
            {
                Title = "Completed onboarding survey",
                TimeAgo = "1 day ago"
            },
            new ActivityItem
            {
                Title = "Requested time off",
                TimeAgo = "2 days ago"
            },
        };
    }

    public override async Task OnNavigatedToAsync()
    {
        // TODO: Load real data from services
        // await LoadDashboardDataAsync();
        
        await base.OnNavigatedToAsync();
    }

    #region Navigation Commands

    [RelayCommand]
    private async Task NavigateToDashboard()
    {
        SelectedMenuItem = "Dashboard";
        // Already on dashboard, could refresh data
        await Task.CompletedTask;
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
        // TODO: Navigate to time off view when implemented
        // await _navigationService.NavigateTo(ViewModelType.Attendance);
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
public class TaskItem
{
    public string Title { get; set; } = "";
    public string DueDate { get; set; } = "";
}
