using System.Threading.Tasks;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.Utils.Enums;

namespace Client.ViewModels;

/// <summary>
/// ViewModel for the Intern Portal Dashboard
/// Provides access to intern-specific features like profile, attendance, tasks, and time off requests
/// </summary>
public partial class InternDashboardViewModel(INavigationService navigationService) : ViewModelBase
{
    private readonly INavigationService _navigationService = navigationService;

    [ObservableProperty]
    private string _welcomeMessage = "Welcome to Intern Portal";

    [ObservableProperty]
    private string _portalDescription = "View your profile, attendance, tasks, and time off";

    public override async Task OnNavigatedToAsync()
    {
        // Initialize Intern Dashboard data
        // TODO: Load intern profile, attendance records, assigned tasks, etc.
        await base.OnNavigatedToAsync();
    }

    // TODO: Add commands for:
    // - View/edit profile
    // - Check attendance
    // - View assigned tasks
    // - Request time off
    // - View time off history
    [RelayCommand]
    private async Task GoToMyProfile()
    {
        await _navigationService.NavigateTo(ViewModelType.InternDashboard); // Replace with MyProfile if available
    }

    [RelayCommand]
    private async Task GoToMetrics()
    {
        await _navigationService.NavigateTo(ViewModelType.InternDashboard); // Replace with MetricsViewModel
    }

    [RelayCommand]
    private async Task GoToTasks()
    {
        await _navigationService.NavigateTo(ViewModelType.InternDashboard); // Replace with TasksViewModel
    }
}

