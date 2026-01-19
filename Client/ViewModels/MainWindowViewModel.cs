using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.Services;
using Client.Utils.Classes;
using Client.Utils.Enums;
using Client.Utils.Interfaces;
using Client.Models.EmployeeManagement;

namespace Client.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly IAuthenticationService _authService;
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly IApiClient _apiClient;
    private readonly ISessionService _sessionService;
    private readonly SidebarViewModel _sidebarViewModel;
    private readonly IFileService _fileService;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IInvitationService _invitationService;

    [ObservableProperty]
    private ViewModelBase? _currentViewModel;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDevUser))]
    [NotifyPropertyChangedFor(nameof(IsDevButtonVisible))]
    [NotifyPropertyChangedFor(nameof(IsSwitchPortalVisible))]
    private bool _isAuthenticated;

    // Controls sidebar visibility per-view
    [ObservableProperty]
    private bool _isSidebarVisible;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDevButtonVisible))]
    [NotifyPropertyChangedFor(nameof(IsSwitchPortalVisible))]
    private bool _isDevModeEnabled = true;
    
    [ObservableProperty]
    private bool _isComingSoonOpen;

    [ObservableProperty]
    private string _devModeButtonText = "Disable Dev Mode";

    public bool IsDevUser =>
        _authService.CurrentUserEmail != null &&
        (_authService.CurrentUserEmail.Equals("luis.orbe.projxon@gmail.com", StringComparison.OrdinalIgnoreCase) ||
         _authService.CurrentUserEmail.Equals("annalysa.vicci.projxon@gmail.com", StringComparison.OrdinalIgnoreCase));

    public bool IsDevButtonVisible => IsDevUser && IsDevModeEnabled;
    
    public bool IsSwitchPortalVisible => 
        IsAuthenticated && 
        ((IsDevUser && IsDevModeEnabled) || _sessionService.IsHrOrExecutive);

    public SidebarViewModel SidebarViewModel => _sidebarViewModel;

    public MainWindowViewModel(
        INavigationService navigationService,
        IAuthenticationService authService,
        IEmployeeRepository employeeRepository,
        IUserPreferencesService userPreferencesService,
        IApiClient apiClient,
        ISessionService sessionService,
        SidebarViewModel sidebarViewModel,
        IFileService fileService,
        IInvitationService invitationService)
    {
        _navigationService = navigationService;
        _authService = authService;
        _employeeRepository = employeeRepository;
        _userPreferencesService = userPreferencesService;
        _apiClient = apiClient;
        _sessionService = sessionService;
        _sidebarViewModel = sidebarViewModel;
        _fileService = fileService;
        _invitationService = invitationService;

        _navigationService.NavigationRequested += OnNavigationRequested;
        _authService.AuthenticationChanged += OnIsAuthenticatedChanged;
        
        _sidebarViewModel.ComingSoonRequested += (s, e) => IsComingSoonOpen = true;

        InitializeView();
    }

    private void InitializeView()
    {
        IsAuthenticated = _authService.IsAuthenticated;

        if (IsAuthenticated)
        {
            CurrentViewModel = new PortalSelectionViewModel(_navigationService, _userPreferencesService);
            IsSidebarVisible = false;
        }
        else
        {
            CurrentViewModel = new LoginViewModel(_authService);
            IsSidebarVisible = false;
        }
    }

    private async Task OnNavigationRequested(object? sender, NavigationEventArgs e)
    {
        ViewModelBase? newVm = null;

        // 1. Determine Logic based on ViewModel Type
        switch (e.ViewModelType)
        {
            case ViewModelType.Login:
                newVm = new LoginViewModel(_authService);
                IsSidebarVisible = false;
                break;

            case ViewModelType.PortalSelection:
                newVm = new PortalSelectionViewModel(_navigationService, _userPreferencesService);
                IsSidebarVisible = false;
                break;

            // For all Dashboards and Detail views, SHOW Sidebar
            case ViewModelType.HRDashboard:
                newVm = new HRDashboardViewModel(_navigationService, _userPreferencesService, _employeeRepository, _sessionService, _apiClient, _sidebarViewModel);
                IsSidebarVisible = true;
                break;
            case ViewModelType.InternDashboard:
                newVm = new InternDashboardViewModel(_navigationService, _sessionService, _apiClient, _sidebarViewModel);
                IsSidebarVisible = true;
                break;
            case ViewModelType.Dashboard:
                newVm = new DashboardViewModel(_navigationService);
                IsSidebarVisible = true;
                break;
            case ViewModelType.EmployeesList:
                newVm = new EmployeesListViewModel(_employeeRepository, _navigationService);
                IsSidebarVisible = true;
                break;
            case ViewModelType.Employees:
                newVm = new EmployeesViewModel(_employeeRepository, _navigationService, _sidebarViewModel, _invitationService);
                IsSidebarVisible = true;
                break;
            case ViewModelType.EmployeeDetails:
                newVm = CreateEmployeeDetailViewModel(e.EntityId);
                IsSidebarVisible = true;
                break;
            case ViewModelType.Profile:
                newVm = new ProfileViewModel(
                    _navigationService, 
                    _sessionService, 
                    _employeeRepository, 
                    _fileService, 
                    _apiClient, 
                    _sidebarViewModel,
                    _userPreferencesService,
                    null);
                IsSidebarVisible = true;
                break;
            case ViewModelType.Attendance:
                newVm = new AttendanceViewModel(_navigationService, _sidebarViewModel, _userPreferencesService, _apiClient, _sessionService);
                IsSidebarVisible = true;
                break;
            case ViewModelType.Recruitment:
                newVm = new RecruitmentViewModel(_navigationService, _apiClient, _sidebarViewModel);
                IsSidebarVisible = true;
                break;
            case ViewModelType.Forms:
                newVm = new FormsViewModel(_navigationService, _sidebarViewModel, _apiClient, _employeeRepository);
                IsSidebarVisible = true;
                break;
            case ViewModelType.CreateSurvey:
                newVm = new CreateSurveyViewModel(_navigationService, _sidebarViewModel, _apiClient);
                IsSidebarVisible = true;
                break;
            case ViewModelType.Tasks:
                newVm = new TasksViewModel(_sessionService, _apiClient, _sidebarViewModel, _navigationService);
                IsSidebarVisible = true;
                break;
            case ViewModelType.TakeSurvey:
                var takeSurveyVm = new TakeSurveyViewModel(_apiClient, _navigationService);
                if (e.EntityId > 0) await takeSurveyVm.LoadFromAssignmentAsync(e.EntityId);
                newVm = takeSurveyVm;
                IsSidebarVisible = true;
                break;
                
            default:
                newVm = CurrentViewModel;
                break;
        }

        if (newVm is null) return;
        if (CurrentViewModel is not null) await CurrentViewModel.OnNavigatedFromAsync();
        
        CurrentViewModel = newVm;
        await CurrentViewModel.OnNavigatedToAsync();
    }

    private EmployeeDetailViewModel CreateEmployeeDetailViewModel(int employeeId)
    {
        var vm = new EmployeeDetailViewModel(_navigationService, _sidebarViewModel, _employeeRepository);
        vm.SetEmployeeId(employeeId);
        return vm;
    }

    private async void OnIsAuthenticatedChanged(object? sender, AuthenticationChangedEventArgs e)
    {
        IsAuthenticated = e.IsAuthenticated;
        OnPropertyChanged(nameof(IsSwitchPortalVisible)); 
        OnPropertyChanged(nameof(IsDevButtonVisible)); 

        if (IsAuthenticated)
        {
            if (IsDevUser)
            {
                await _navigationService.NavigateTo(ViewModelType.PortalSelection);
                return;
            }

            if (_sessionService.IsHrOrExecutive)
            {
                await _navigationService.NavigateTo(ViewModelType.PortalSelection);
            }
            else
            {
                await _userPreferencesService.SetPortalPreferenceAsync(PortalType.Intern);
                await _navigationService.NavigateTo(ViewModelType.InternDashboard);
            }
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
    private async Task NavigateToDashboard() => await _navigationService.NavigateTo(ViewModelType.Dashboard);
    
    [RelayCommand]
    private async Task NavigateToEmployeesList() => await _navigationService.NavigateTo(ViewModelType.Employees);
    
    [RelayCommand]
    private async Task Logout() => await _authService.LogoutAsync();
    
    [RelayCommand]
    private void CloseComingSoon() => IsComingSoonOpen = false;
}