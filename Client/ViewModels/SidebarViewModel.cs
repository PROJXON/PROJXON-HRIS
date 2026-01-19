using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using Client.Services;
using Client.Utils.Classes;
using Client.Utils.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.EmployeeManagement.Responses;

using ServiceSessionArgs = Client.Services.SessionChangedEventArgs;

namespace Client.ViewModels;

public partial class SidebarViewModel : ViewModelBase
{
    private readonly ISessionService _sessionService;
    private readonly IFileService _fileService;
    private readonly INavigationService _navigationService;

    private const string BaseUrl = "http://localhost:8080";

    [ObservableProperty]
    private string _userName = string.Empty;

    [ObservableProperty]
    private string _userRole = string.Empty;

    [ObservableProperty]
    private string? _profilePictureUrl;

    [ObservableProperty]
    private bool _isUploadingProfilePicture;

    [ObservableProperty]
    private string _currentPage = string.Empty;

    public SidebarViewModel(
        ISessionService sessionService,
        IFileService fileService,
        INavigationService navigationService)
    {
        _sessionService = sessionService;
        _fileService = fileService;
        _navigationService = navigationService;

        if (_sessionService != null)
        {
            _sessionService.SessionChanged += OnSessionChanged;
            
            // Initialize if session already exists (e.g. app reload or navigation return)
            if (_sessionService.CurrentEmployee != null)
            {
                LoadFromEmployee(_sessionService.CurrentEmployee);
            }
        }
    }

    // Default constructor for design-time
    public SidebarViewModel() : this(null!, null!, null!)
    {
    }

    private void OnSessionChanged(object? sender, ServiceSessionArgs e)
    {
        // Handle both Updated (refresh/upload) AND Initialized (login)
        if ((e.ChangeType == SessionChangeType.Updated || e.ChangeType == SessionChangeType.Initialized) 
            && e.Employee != null)
        {
            Dispatcher.UIThread.Post(() => LoadFromEmployee(e.Employee));
        }
        // Clear data on logout
        else if (e.ChangeType == SessionChangeType.Cleared)
        {
            Dispatcher.UIThread.Post(() => {
                UserName = string.Empty;
                UserRole = string.Empty;
                ProfilePictureUrl = null;
            });
        }
    }

    private void LoadFromEmployee(EmployeeResponse employee)
    {
        // Display name should be first + last name
        var first = employee.BasicInfo?.FirstName ?? string.Empty;
        var last = employee.BasicInfo?.LastName ?? string.Empty;
        UserName = $"{first} {last}".Trim();

        // Mapping PositionName from DTO to UserRole
        var jobTitle = employee.PositionDetails?.PositionName ?? string.Empty;
        UserRole = string.IsNullOrWhiteSpace(jobTitle) ? "Employee" : jobTitle;

        ProfilePictureUrl = SanitizeServerUrl(employee.Documents?.ProfilePictureUrl);
    }

    private string? SanitizeServerUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return null;
        if (url.StartsWith(BaseUrl)) return url;
        if (url.StartsWith("/")) return $"{BaseUrl}{url}";
        if (url.Contains(":8080") && !url.Contains("localhost"))
        {
            var uri = new Uri(url);
            return $"{BaseUrl}{uri.PathAndQuery}";
        }
        return url;
    }

    [RelayCommand]
    private async Task UploadProfilePictureAsync()
    {
        var employeeId = _sessionService.CurrentEmployee?.Id ?? 0;
        if (employeeId == 0) return;

        try
        {
            var file = await _fileService.PickImageAsync("Select Profile Picture");
            if (file == null) return;

            IsUploadingProfilePicture = true;

            // Using "profile-picture" (hyphenated) to match Backend logic
            var result = await _fileService.UploadFileAsync(file, employeeId, "profile-picture");

            if (result.Success && result.FileUrl != null)
            {
                // We rely on SessionService refresh to update the UI via SessionChanged event
                // This triggers OnSessionChanged with SessionChangeType.Updated
                await _sessionService.RefreshEmployeeDataAsync();
            }
        }
        catch (Exception)
        {
            // ideally log
        }
        finally
        {
            IsUploadingProfilePicture = false;
        }
    }

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
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task NavigateToAttendance()
    {
        await _navigationService.NavigateTo(ViewModelType.Attendance);
    }

    [RelayCommand]
    private async Task NavigateToEmployees()
    {
        await _navigationService.NavigateTo(ViewModelType.Employees);
    }

    [RelayCommand]
    private async Task NavigateToRecruitment()
    {
        await _navigationService.NavigateTo(ViewModelType.Recruitment);
    }

    [RelayCommand]
    private async Task NavigateToForms()
    {
        await _navigationService.NavigateTo(ViewModelType.Forms);
    }

    #endregion

    public override async Task OnNavigatedFromAsync()
    {
        await base.OnNavigatedFromAsync();
        if (_sessionService != null)
        {
            _sessionService.SessionChanged -= OnSessionChanged;
        }
    }
}