using System.Threading.Tasks;
using Client.Services;
using Client.Utils.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia;

namespace Client.ViewModels;

public partial class PortalSelectionViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IUserPreferencesService _userPreferencesService;

    public PortalSelectionViewModel(
        INavigationService navigationService,
        IUserPreferencesService userPreferencesService)
    {
        _navigationService = navigationService;
        _userPreferencesService = userPreferencesService;
    }

    [RelayCommand]
    private async Task SelectHRPortal()
    {
        // Store portal preference
        await _userPreferencesService.SetPortalPreferenceAsync(PortalType.HR);
        
        // Navigate to HR Dashboard
        await _navigationService.NavigateTo(ViewModelType.HRDashboard);
    }

    [RelayCommand]
    private async Task SelectInternPortal()
    {
        // Store portal preference
        await _userPreferencesService.SetPortalPreferenceAsync(PortalType.Intern);
        
        // Navigate to Intern Dashboard
        await _navigationService.NavigateTo(ViewModelType.InternDashboard);
    }

    public override async Task OnNavigatedToAsync()
    {
        var mainVm = (MainWindowViewModel?)Application.Current?.DataContext;
        var isDev = mainVm?.IsDevUser ?? false;

        // If it's a dev user, DO NOT auto-navigate. Let them choose.
        if (!isDev)
        {
            // Check if user has a stored portal preference
            var savedPortal = await _userPreferencesService.GetPortalPreferenceAsync();
            
            if (savedPortal.HasValue)
            {
                var targetViewModel = savedPortal.Value == PortalType.HR 
                    ? ViewModelType.HRDashboard 
                    : ViewModelType.InternDashboard;
                
                await _navigationService.NavigateTo(targetViewModel);
            }
        }
        
        await base.OnNavigatedToAsync();
    }
}
