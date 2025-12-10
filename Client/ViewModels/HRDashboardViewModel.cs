<<<<<<< HEAD
using System.Threading.Tasks;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
=======
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Client.Services;
using Client.Utils.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
>>>>>>> origin/main

namespace Client.ViewModels;

/// <summary>
/// ViewModel for the HR Portal Dashboard
/// Provides access to HR-specific features like employee management, recruitment, time off approval, etc.
/// </summary>
public partial class HRDashboardViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

<<<<<<< HEAD
    [ObservableProperty]
    private string _welcomeMessage = "Welcome to HR Portal";

    [ObservableProperty]
    private string _portalDescription = "Manage employees, recruitment, time off, and more";
=======
    #region User Profile Properties

    [ObservableProperty]
    private string _userName = "John Smith";

    [ObservableProperty]
    private string _userRole = "HR Manager";

    #endregion

    #region Dashboard Statistics

    [ObservableProperty]
    private int _totalEmployees = 48;

    [ObservableProperty]
    private int _pendingTimeOff = 12;

    [ObservableProperty]
    private int _activeRecruitments = 5;

    [ObservableProperty]
    private int _attendancePercentage = 96;

    #endregion

    #region Recent Activity

    [ObservableProperty]
    private ObservableCollection<ActivityItem> _recentActivities = new();

    #endregion

    #region Selected Menu Item

    [ObservableProperty]
    private string _selectedMenuItem = "Dashboard";

    #endregion
>>>>>>> origin/main

    public HRDashboardViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
<<<<<<< HEAD
=======
        LoadRecentActivities();
    }

    private void LoadRecentActivities()
    {
        RecentActivities = new ObservableCollection<ActivityItem>
        {
            new ActivityItem
            {
                Title = "New time off request",
                Subtitle = "John Smith",
                TimeAgo = "2 hours ago"
            },
            new ActivityItem
            {
                Title = "Employee onboarded",
                Subtitle = "Sarah Johnson joined Marketing",
                TimeAgo = "5 hours ago"
            },
            new ActivityItem
            {
                Title = "Time off approved",
                Subtitle = "Mike Davis - Vacation",
                TimeAgo = "Yesterday"
            },
            new ActivityItem
            {
                Title = "New recruitment posted",
                Subtitle = "Senior Developer position",
                TimeAgo = "2 days ago"
            }
        };
>>>>>>> origin/main
    }

    public override async Task OnNavigatedToAsync()
    {
<<<<<<< HEAD
        // Initialize HR Dashboard data
        // TODO: Load HR-specific widgets, statistics, pending approvals, etc.
=======
        // TODO: Load real data from services
        // await LoadDashboardDataAsync();
>>>>>>> origin/main
        
        await base.OnNavigatedToAsync();
    }

<<<<<<< HEAD
    // TODO: Add commands for:
    // - Navigate to employee management
    // - View pending time-off requests
    // - Access recruitment pipeline
    // - View reports and analytics
=======
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
        SelectedMenuItem = "Profile";
        // TODO: Navigate to profile view when implemented
        // await _navigationService.NavigateTo(ViewModelType.Profile);
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task NavigateToTimeOff()
    {
        SelectedMenuItem = "Time Off Requests";
        // TODO: Navigate to time off view when implemented
        // await _navigationService.NavigateTo(ViewModelType.TimeOffRequests);
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task NavigateToAttendance()
    {
        SelectedMenuItem = "Attendance";
        // TODO: Navigate to attendance view when implemented
        // await _navigationService.NavigateTo(ViewModelType.Attendance);
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task NavigateToEmployees()
    {
        SelectedMenuItem = "Employees";
        await _navigationService.NavigateTo(ViewModelType.EmployeesList);
    }

    #endregion
}

/// <summary>
/// Model for activity feed items
/// </summary>
public class ActivityItem
{
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string TimeAgo { get; set; } = string.Empty;
>>>>>>> origin/main
}
