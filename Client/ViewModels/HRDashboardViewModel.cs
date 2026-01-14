using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Client.Models;
using Client.Services;
using Client.Utils.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Client.ViewModels;
using Client.Models;

/// <summary>
/// ViewModel for the HR Portal Dashboard
/// Provides access to HR-specific features like employee management, recruitment, time off approval, etc.
/// </summary>
public partial class HRDashboardViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

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

    public HRDashboardViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        LoadRecentActivities();
    }

    // Parameterless constructor for design-time support
    public HRDashboardViewModel() : this(null!)
    {
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
        SelectedMenuItem = "Profile";
        await _navigationService.NavigateTo(ViewModelType.Profile);
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
        await _navigationService.NavigateTo(ViewModelType.Attendance);
    }

    [RelayCommand]
    private async Task NavigateToEmployees()
    {
        SelectedMenuItem = "Employees";
        await _navigationService.NavigateTo(ViewModelType.Employees);
    }

    [RelayCommand]
    private async Task NavigateToRecruitment()
    {
        SelectedMenuItem = "Recruitment";
        await _navigationService.NavigateTo(ViewModelType.Recruitment);
    }

    [RelayCommand]
    private async Task NavigateToForms()
    {
        SelectedMenuItem = "Forms";
        await _navigationService.NavigateTo(ViewModelType.Forms);
    }

    #endregion
}
