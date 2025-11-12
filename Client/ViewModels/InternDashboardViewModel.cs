using System.Threading.Tasks;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Client.ViewModels;

/// <summary>
/// ViewModel for the Intern Portal Dashboard
/// Provides access to intern-specific features like profile, attendance, tasks, and time off requests
/// </summary>
public partial class InternDashboardViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _welcomeMessage = "Welcome to Intern Portal";

    [ObservableProperty]
    private string _portalDescription = "View your profile, attendance, tasks, and time off";

    public InternDashboardViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

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
}
