using System.Threading.Tasks;
using Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Client.ViewModels;

/// <summary>
/// ViewModel for the HR Portal Dashboard
/// Provides access to HR-specific features like employee management, recruitment, time off approval, etc.
/// </summary>
public partial class HRDashboardViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _welcomeMessage = "Welcome to HR Portal";

    [ObservableProperty]
    private string _portalDescription = "Manage employees, recruitment, time off, and more";

    public HRDashboardViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public override async Task OnNavigatedToAsync()
    {
        // Initialize HR Dashboard data
        // TODO: Load HR-specific widgets, statistics, pending approvals, etc.
        
        await base.OnNavigatedToAsync();
    }

    // TODO: Add commands for:
    // - Navigate to employee management
    // - View pending time-off requests
    // - Access recruitment pipeline
    // - View reports and analytics
}
