using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.Services;
using Client.Utils.Classes;
using Client.Utils.Enums;

namespace Client.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IAuthenticationService _authService;

    [ObservableProperty]
    private ViewModelBase? _currentViewModel;

    [ObservableProperty]
    private bool _isAuthenticated;

    public MainWindowViewModel(INavigationService navigationService, IAuthenticationService authService)
    {
        _navigationService = navigationService;
        _authService = authService;

        _navigationService.NavigationRequested += OnNavigationRequested;
        _authService.AuthenticationChanged += OnIsAuthenticatedChanged;

        InitializeView();
    }

    private void InitializeView()
    {
        IsAuthenticated = true; // TODO set this to _isAuthenticated when implementing login view

        if (IsAuthenticated)
        {
            CurrentViewModel = new DashboardViewModel(_navigationService);
        }
        else
        {
            CurrentViewModel = new LoginViewModel(_authService);
        }
    }

    private void OnNavigationRequested(object? sender, NavigationEventArgs e)
    {
        CurrentViewModel = e.ViewModelType switch
        {
            ViewModelType.Login => new LoginViewModel(_authService),
            ViewModelType.Dashboard => new DashboardViewModel(_navigationService),
            _ => CurrentViewModel
        };
    }

    private void OnIsAuthenticatedChanged(object? sender, AuthenticationChangedEventArgs e)
    {
        IsAuthenticated = e.IsAuthenticated;

        _navigationService.NavigateTo(e.IsAuthenticated ? ViewModelType.Dashboard : ViewModelType.Login);
    }

    [RelayCommand]
    private void NavigateToDashboard()
    {
        _navigationService.NavigateTo(ViewModelType.Dashboard);
    }

    [RelayCommand]
    private async Task Logout()
    {
        await _authService.LogoutAsync();
    }
}