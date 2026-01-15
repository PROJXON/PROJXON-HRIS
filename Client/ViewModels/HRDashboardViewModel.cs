using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Client.Models;
using Client.Models.EmployeeManagement;
using Client.Services;
using Client.Utils.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.EmployeeManagement.Responses;

namespace Client.ViewModels;

public partial class HRDashboardViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly IEmployeeRepository _employeeRepository;

    [ObservableProperty]
    private string _userName = "John Smith";

    [ObservableProperty]
    private string _userRole = "HR Manager";

    [ObservableProperty]
    private int _totalEmployees;

    [ObservableProperty]
    private int _pendingTimeOff = 0;

    [ObservableProperty]
    private int _activeRecruitments = 0;

    [ObservableProperty]
    private int _attendancePercentage = 0;

    [ObservableProperty]
    private ObservableCollection<ActivityItem> _recentActivities = new();

    [ObservableProperty]
    private string _selectedMenuItem = "Dashboard";

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

    public bool DevModeToggle
    {
        get => ((MainWindowViewModel)Application.Current!.DataContext!).IsDevModeEnabled;
        set
        {
            ((MainWindowViewModel)Application.Current!.DataContext!).IsDevModeEnabled = value;
            OnPropertyChanged(nameof(ShowDevOptions));
            OnPropertyChanged(nameof(DevModeToggle));
        }
    }

    public HRDashboardViewModel(
        INavigationService navigationService,
        IUserPreferencesService userPreferencesService,
        IEmployeeRepository employeeRepository)
    {
        _navigationService = navigationService;
        _userPreferencesService = userPreferencesService;
        _employeeRepository = employeeRepository;
    }

    public HRDashboardViewModel() : this(null!, null!, null!)
    {
    }

    public override async Task OnNavigatedToAsync()
    {
        await LoadDashboardDataAsync();
        await base.OnNavigatedToAsync();
    }

    private async Task LoadDashboardDataAsync()
    {
        var result = await _employeeRepository.GetAllAsync<EmployeeResponse>();

        if (result.IsSuccess && result.Value != null)
        {
            var employees = result.Value.ToList();

            TotalEmployees = employees.Count;

            RecentActivities.Clear();

            var recentHires = employees
                .OrderByDescending(e => e.CreateDateTime)
                .Take(5);

            foreach (var emp in recentHires)
            {
                var name = $"{emp.BasicInfo?.FirstName} {emp.BasicInfo?.LastName}".Trim();
                var created = emp.CreateDateTime ?? DateTime.UtcNow;

                RecentActivities.Add(new ActivityItem
                {
                    Title = "New Employee Hired",
                    Subtitle = $"{name} joined the team",
                    TimeAgo = GetTimeAgo(created)
                });
            }
        }
    }

    private string GetTimeAgo(DateTime date)
    {
        var span = DateTime.UtcNow - date;
        if (span.TotalMinutes < 60) return $"{span.TotalMinutes:F0} mins ago";
        if (span.TotalHours < 24) return $"{span.TotalHours:F0} hours ago";
        return $"{span.TotalDays:F0} days ago";
    }

    [RelayCommand]
    private async Task NavigateToDashboard()
    {
        SelectedMenuItem = "Dashboard";
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

    [RelayCommand]
    private async Task SwitchToInternPortal()
    {
        await _userPreferencesService.ClearPortalPreferenceAsync();
        await _navigationService.NavigateTo(ViewModelType.InternDashboard);
    }
}
