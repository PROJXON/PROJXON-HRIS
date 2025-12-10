using System;
using System.Threading.Tasks;
using Client.Models.EmployeeManagement;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.Services;
using Client.Utils.Classes;
using Client.Utils.Enums;
using Client.Utils.Interfaces;

namespace Client.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly IAuthenticationService _authService;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUserPreferencesService _userPreferencesService;

    [ObservableProperty]
    private ViewModelBase? _currentViewModel;

    [ObservableProperty]
    private bool _isAuthenticated;

    public MainWindowViewModel(
        INavigationService navigationService, 
        IAuthenticationService authService, 
        IEmployeeRepository employeeRepository,
        IUserPreferencesService userPreferencesService)
    {
        _navigationService = navigationService;
        _authService = authService;
        _employeeRepository = employeeRepository;
        _userPreferencesService = userPreferencesService;

        _navigationService.NavigationRequested += OnNavigationRequested;
        _authService.AuthenticationChanged += OnIsAuthenticatedChanged;

        InitializeView();
    }

    private void InitializeView()
    {
        IsAuthenticated = _authService.IsAuthenticated;

        if (IsAuthenticated)
        {
            // Show portal selection after login
            CurrentViewModel = new PortalSelectionViewModel(_navigationService, _userPreferencesService);
        }
        else
        {
            CurrentViewModel = new LoginViewModel(_authService);
        }
    }

    private async Task OnNavigationRequested(object? sender, NavigationEventArgs e)
    {
        var newVm = e.ViewModelType switch
        {
            ViewModelType.Login => new LoginViewModel(_authService),
            ViewModelType.PortalSelection => new PortalSelectionViewModel(_navigationService, _userPreferencesService),
<<<<<<< HEAD
            ViewModelType.HRDashboard => new DashboardViewModel(_navigationService), // TODO: Create HRDashboardViewModel
            ViewModelType.InternDashboard => new InternDashboardViewModel(_navigationService), // TODO: Create InternDashboardViewModel
=======
            ViewModelType.HRDashboard => new HRDashboardViewModel(_navigationService),
            ViewModelType.InternDashboard => new InternDashboardViewModel(_navigationService),
>>>>>>> origin/main
            ViewModelType.Dashboard => new DashboardViewModel(_navigationService),
            ViewModelType.EmployeesList => new EmployeesListViewModel(_employeeRepository, _navigationService),
            _ => CurrentViewModel
        };

        if (newVm is null)
        {
            return;
        }

        // Call lifecycle methods
        if (CurrentViewModel is not null)
        {
            await CurrentViewModel.OnNavigatedFromAsync();
        }

        CurrentViewModel = newVm;
        await CurrentViewModel.OnNavigatedToAsync();
    }

    private async void OnIsAuthenticatedChanged(object? sender, AuthenticationChangedEventArgs e)
    {
        IsAuthenticated = e.IsAuthenticated;

        if (IsAuthenticated)
        {
            // Navigate to portal selection when authenticated
            await _navigationService.NavigateTo(ViewModelType.PortalSelection);
        }
        else
        {
            // Clear portal preference on logout
            await _userPreferencesService.ClearPortalPreferenceAsync();
            await _navigationService.NavigateTo(ViewModelType.Login);
        }
    }

    [RelayCommand]
    private async Task NavigateToDashboard()
    {
        await _navigationService.NavigateTo(ViewModelType.Dashboard);
    }

    [RelayCommand]
    private async Task NavigateToEmployeesList()
    {
        await _navigationService.NavigateTo(ViewModelType.EmployeesList);
    }

    [RelayCommand]
    private async Task Logout()
    {
        await _authService.LogoutAsync();
    }
}
