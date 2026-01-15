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
    private readonly IApiClient _apiClient;

    [ObservableProperty]
    private ViewModelBase? _currentViewModel;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDevUser))]
    [NotifyPropertyChangedFor(nameof(IsDevButtonVisible))]
    [NotifyPropertyChangedFor(nameof(IsSwitchPortalVisible))]
    private bool _isAuthenticated;

    // Dev Mode State
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDevButtonVisible))]
    [NotifyPropertyChangedFor(nameof(IsSwitchPortalVisible))]
    private bool _isDevModeEnabled = true;

    [ObservableProperty]
    private string _devModeButtonText = "Disable Dev Mode";

    // Strict email check for production prep
    public bool IsDevUser => 
        _authService.CurrentUserEmail != null && 
        (_authService.CurrentUserEmail.Equals("luis.orbe.projxon@gmail.com", StringComparison.OrdinalIgnoreCase) || 
         _authService.CurrentUserEmail.Equals("annalysa.vicci.projxon@gmail.com", StringComparison.OrdinalIgnoreCase));

    // Visibility Logic: Dev button only shows for Dev Users when mode is enabled
    public bool IsDevButtonVisible => IsDevUser && IsDevModeEnabled;

    // Visibility Logic: Switch Portal shows if Authenticated AND Dev Mode is enabled
    // (This allows devs to hide the floating button by disabling dev mode)
    public bool IsSwitchPortalVisible => IsAuthenticated && IsDevModeEnabled;

    public MainWindowViewModel(
        INavigationService navigationService, 
        IAuthenticationService authService, 
        IEmployeeRepository employeeRepository,
        IUserPreferencesService userPreferencesService,
        IApiClient apiClient) 
    {
        _navigationService = navigationService;
        _authService = authService;
        _employeeRepository = employeeRepository;
        _userPreferencesService = userPreferencesService;
        _apiClient = apiClient;

        _navigationService.NavigationRequested += OnNavigationRequested;
        _authService.AuthenticationChanged += OnIsAuthenticatedChanged;

        InitializeView();
    }

    private void InitializeView()
    {
        IsAuthenticated = _authService.IsAuthenticated;

        if (IsAuthenticated)
        {
            CurrentViewModel = new PortalSelectionViewModel(_navigationService, _userPreferencesService);
        }
        else
        {
            CurrentViewModel = new LoginViewModel(_authService);
        }
    }

    private async Task OnNavigationRequested(object? sender, NavigationEventArgs e)
    {
        ViewModelBase? newVm = e.ViewModelType switch
        {
            ViewModelType.Login => new LoginViewModel(_authService),
            ViewModelType.PortalSelection => new PortalSelectionViewModel(_navigationService, _userPreferencesService),
            ViewModelType.HRDashboard => new HRDashboardViewModel(_navigationService, _userPreferencesService, _employeeRepository),
            ViewModelType.InternDashboard => new InternDashboardViewModel(_navigationService),
            ViewModelType.Dashboard => new DashboardViewModel(_navigationService),
            ViewModelType.EmployeesList => new EmployeesListViewModel(_employeeRepository, _navigationService),
            ViewModelType.Employees => new EmployeesViewModel(_employeeRepository, _navigationService),
            ViewModelType.EmployeeDetails => CreateEmployeeDetailViewModel(e.EntityId),
            ViewModelType.Profile => new ProfileViewModel(_navigationService),
            ViewModelType.Attendance => new AttendanceViewModel(_navigationService),
            ViewModelType.Recruitment => new RecruitmentViewModel(_navigationService, _apiClient),
            ViewModelType.Forms => new FormsViewModel(_navigationService),
            ViewModelType.CreateSurvey => new CreateSurveyViewModel(_navigationService),
            _ => CurrentViewModel
        };

        if (newVm is null) return;

        if (CurrentViewModel is not null)
        {
            await CurrentViewModel.OnNavigatedFromAsync();
        }

        CurrentViewModel = newVm;
        await CurrentViewModel.OnNavigatedToAsync();
    }

    private EmployeeDetailViewModel CreateEmployeeDetailViewModel(int employeeId)
    {
        var vm = new EmployeeDetailViewModel(_navigationService, _employeeRepository);
        vm.SetEmployeeId(employeeId);
        return vm;
    }

    private async void OnIsAuthenticatedChanged(object? sender, AuthenticationChangedEventArgs e)
    {
        IsAuthenticated = e.IsAuthenticated;
        
        if (IsAuthenticated)
        {
            await _navigationService.NavigateTo(ViewModelType.PortalSelection);
        }
        else
        {
            await _userPreferencesService.ClearPortalPreferenceAsync();
            await _navigationService.NavigateTo(ViewModelType.Login);
        }
    }

    [RelayCommand]
    private void ToggleDevMode()
    {
        IsDevModeEnabled = !IsDevModeEnabled;
        DevModeButtonText = IsDevModeEnabled ? "Disable Dev Mode" : "Enable Dev Mode";
    }

    [RelayCommand]
    private async Task SwitchPortal()
    {
        var currentPref = await _userPreferencesService.GetPortalPreferenceAsync();
        
        if (currentPref == PortalType.HR)
        {
            await _userPreferencesService.SetPortalPreferenceAsync(PortalType.Intern);
            await _navigationService.NavigateTo(ViewModelType.InternDashboard);
        }
        else
        {
            await _userPreferencesService.SetPortalPreferenceAsync(PortalType.HR);
            await _navigationService.NavigateTo(ViewModelType.HRDashboard);
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
        await _navigationService.NavigateTo(ViewModelType.Employees);
    }

    [RelayCommand]
    private async Task Logout()
    {
        await _authService.LogoutAsync();
    }
}