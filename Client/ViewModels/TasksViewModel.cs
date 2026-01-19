using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using Client.Services;
using Client.Utils.Classes;
using Client.Utils.Enums;
using Client.Utils.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.EmployeeManagement.Responses;

namespace Client.ViewModels;

public partial class TasksViewModel : ViewModelBase
{
    private readonly ISessionService _sessionService;
    private readonly IApiClient _apiClient;
    private readonly INavigationService _navigationService;
    
    public SidebarViewModel Sidebar { get; }

    [ObservableProperty]
    private ObservableCollection<TaskItemResponse> _pendingTasks = new();

    [ObservableProperty]
    private ObservableCollection<TaskItemResponse> _completedTasks = new();

    [ObservableProperty]
    private bool _isLoading;

    public TasksViewModel(
        ISessionService sessionService,
        IApiClient apiClient,
        SidebarViewModel sidebarViewModel,
        INavigationService navigationService)
    {
        _sessionService = sessionService;
        _apiClient = apiClient;
        Sidebar = sidebarViewModel;
        _navigationService = navigationService;
    }

    // Design time
    public TasksViewModel() : this(null!, null!, new SidebarViewModel(), null!) { }

    public override async Task OnNavigatedToAsync()
    {
        Sidebar.CurrentPage = "Tasks";
        Sidebar.SetPortalMode(true);
        await LoadTasksAsync();
    }

    private async Task LoadTasksAsync()
    {
        IsLoading = true;
        var employeeId = _sessionService.CurrentEmployee?.Id ?? 0;
        
        try 
        {
            var result = await _apiClient.GetAllAsync<IEnumerable<TaskItemResponse>>($"api/Survey/tasks/{employeeId}");
            if (result.IsSuccess && result.Data != null)
            {
                var all = result.Data.ToList();
                
                PendingTasks = new ObservableCollection<TaskItemResponse>(
                    all.Where(t => !t.IsCompleted).OrderBy(t => t.DueDate));
                    
                CompletedTasks = new ObservableCollection<TaskItemResponse>(
                    all.Where(t => t.IsCompleted).OrderByDescending(t => t.CompletedDate));
            }
        }
        finally 
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task CompleteSurvey(TaskItemResponse task)
    {
        if (task == null) return;
        
        // Navigate to TakeSurvey, passing only the AssignmentID
        // TakeSurveyViewModel will fetch the full assignment details (including SurveyId) from the backend
        await _navigationService.NavigateTo(ViewModelType.TakeSurvey, task.Id);
    }
    
    [RelayCommand]
    private async Task ViewResponse(TaskItemResponse task)
    {
        if (task == null) return;
        
        // Navigates to the same view as taking the survey
        // Logic inside TakeSurveyViewModel/View handles read-only state if submitted
        await _navigationService.NavigateTo(ViewModelType.TakeSurvey, task.Id);
    }
}