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
using Shared.CandidateManagement.Responses;
using Shared.EmployeeManagement.Responses;

namespace Client.ViewModels;

public partial class HRDashboardViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ISessionService _sessionService;
    private readonly IApiClient _apiClient;

    public SidebarViewModel Sidebar { get; }

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

    // Update design-time constructor
    public HRDashboardViewModel() : this(null!, null!, null!, null!, null!, new SidebarViewModel())
    {
    }

    public override async Task OnNavigatedToAsync()
    {
        Sidebar.CurrentPage = "Dashboard";
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
        // Load Employees
        var employeeResult = await _employeeRepository.GetAllAsync<EmployeeResponse>();

        if (employeeResult.IsSuccess && employeeResult.Value != null)
        {
            var employees = employeeResult.Value.ToList();
            TotalEmployees = employees.Count;
            
            // Populate Activities
            RecentActivities.Clear();
            var activities = new List<(DateTime Date, string Title, string Subtitle)>();

            foreach (var emp in employees)
            {
                var name = $"{emp.BasicInfo?.FirstName} {emp.BasicInfo?.LastName}".Trim();
                
                if (emp.CreateDateTime.HasValue)
                {
                    activities.Add((emp.CreateDateTime.Value, "New Employee Hired", $"{name} joined the team"));
                }

                if (emp.UpdateDateTime.HasValue && emp.CreateDateTime.HasValue && 
                    (emp.UpdateDateTime.Value - emp.CreateDateTime.Value).TotalMinutes > 5)
                {
                    activities.Add((emp.UpdateDateTime.Value, "Employee Profile Updated", $"{name}'s profile was updated"));
                }
            }

            var recentItems = activities.OrderByDescending(x => x.Date).Take(5);
            foreach (var item in recentItems)
            {
                RecentActivities.Add(new ActivityItem
                {
                    Title = item.Title,
                    Subtitle = item.Subtitle,
                    TimeAgo = GetTimeAgo(item.Date)
                });
            }
        }

        // Load Active Recruitments (New Logic)
        try
        {
            var candidateResult = await _apiClient.GetAllAsync<IEnumerable<CandidateResponse>>("api/candidate");
            if (candidateResult.IsSuccess && candidateResult.Data != null)
            {
                // Filter out candidates who are Hired or Rejected
                ActiveRecruitments = candidateResult.Data.Count(c => 
                    !string.Equals(c.Status, "Hired", StringComparison.OrdinalIgnoreCase) && 
                    !string.Equals(c.Status, "Rejected", StringComparison.OrdinalIgnoreCase));
            }
        }
        catch (Exception)
        {
            // Fail silently on dashboard stats to avoid crashing the view
            ActiveRecruitments = 0;
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
    private async Task SwitchToInternPortal()
    {
        await _userPreferencesService.ClearPortalPreferenceAsync();
        await _navigationService.NavigateTo(ViewModelType.InternDashboard);
    }
}