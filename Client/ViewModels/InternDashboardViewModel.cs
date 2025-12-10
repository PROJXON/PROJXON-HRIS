using System.Threading.Tasks;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
<<<<<<< HEAD
using CommunityToolkit.Mvvm.Input;
using Client.Utils.Enums;
=======
>>>>>>> origin/main

namespace Client.ViewModels;

/// <summary>
/// ViewModel for the Intern Portal Dashboard
/// Provides access to intern-specific features like profile, attendance, tasks, and time off requests
/// </summary>
<<<<<<< HEAD
public partial class InternDashboardViewModel(INavigationService navigationService) : ViewModelBase
{
    private readonly INavigationService _navigationService = navigationService;
=======
public partial class InternDashboardViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
>>>>>>> origin/main

    [ObservableProperty]
    private string _welcomeMessage = "Welcome to Intern Portal";

    [ObservableProperty]
    private string _portalDescription = "View your profile, attendance, tasks, and time off";

<<<<<<< HEAD
=======
    public InternDashboardViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

>>>>>>> origin/main
    public override async Task OnNavigatedToAsync()
    {
        // Initialize Intern Dashboard data
        // TODO: Load intern profile, attendance records, assigned tasks, etc.
<<<<<<< HEAD
=======
        
>>>>>>> origin/main
        await base.OnNavigatedToAsync();
    }

    // TODO: Add commands for:
    // - View/edit profile
    // - Check attendance
    // - View assigned tasks
    // - Request time off
    // - View time off history
<<<<<<< HEAD
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

=======
}
>>>>>>> origin/main
