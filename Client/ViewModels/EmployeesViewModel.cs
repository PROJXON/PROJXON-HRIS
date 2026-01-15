using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using Client.Models.EmployeeManagement;
using Client.Services;
using Client.Utils.Enums;
using Client.Utils.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.EmployeeManagement.Responses;

namespace Client.ViewModels;

public partial class EmployeesViewModel : ViewModelBase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly INavigationService _navigationService;

    [ObservableProperty] private string _userName = "John Smith";
    [ObservableProperty] private string _userRole = "HR Manager";
    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private string _selectedFilter = "All";
    [ObservableProperty] private ObservableCollection<AlphabetFilterItem> _alphabetFilters = new();
    [ObservableProperty] private ObservableCollection<EmployeeCardViewModel> _allEmployees = new();
    [ObservableProperty] private ObservableCollection<EmployeeCardViewModel> _filteredEmployees = new();
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _hasNoResults;

    public EmployeesViewModel(IEmployeeRepository employeeRepository, INavigationService navigationService)
    {
        _employeeRepository = employeeRepository;
        _navigationService = navigationService;
        InitializeAlphabetFilters();
    }

    // Parameterless constructor for design-time support
    public EmployeesViewModel() : this(null!, null!) { }

    private void InitializeAlphabetFilters()
    {
        AlphabetFilters.Clear();
        AlphabetFilters.Add(new AlphabetFilterItem { Letter = "All", IsSelected = true });
        for (char c = 'A'; c <= 'Z'; c++)
        {
            AlphabetFilters.Add(new AlphabetFilterItem { Letter = c.ToString(), IsSelected = false });
        }
    }

    public override async Task OnNavigatedToAsync()
    {
        await LoadEmployeesAsync();
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task LoadEmployeesAsync()
    {
        if (_employeeRepository == null) return;

        IsLoading = true;
        ErrorMessage = string.Empty;
        HasNoResults = false;

        try
        {
            var result = await _employeeRepository.GetAllAsync<EmployeeResponse>();
            if (result.IsSuccess)
            {
                var employees = result.Value?.ToList() ?? [];
                
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    AllEmployees = new ObservableCollection<EmployeeCardViewModel>(
                        employees.Select(e => new EmployeeCardViewModel
                        {
                            Id = e.Id ?? 0,
                            FirstName = e.BasicInfo?.FirstName ?? "Unknown",
                            LastName = e.BasicInfo?.LastName ?? "Unknown",
                            JobTitle = e.PositionDetails?.PositionName ?? "N/A",
                            Email = e.ContactInfo?.ProjxonEmail ?? e.ContactInfo?.PersonalEmail ?? "No Email",
                            DiscordUsername = "N/A", 
                            Department = "General" 
                        }));
                    ApplyFilters();
                });
            }
            else
            {
                ErrorMessage = $"Failed to load: {result.ErrorMessage}";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSearchQueryChanged(string value) => ApplyFilters();
    partial void OnSelectedFilterChanged(string value) => ApplyFilters();

    private void ApplyFilters()
    {
        var filtered = AllEmployees.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var query = SearchQuery.ToLowerInvariant();
            filtered = filtered.Where(e =>
                e.FullName.ToLowerInvariant().Contains(query) ||
                e.Email.ToLowerInvariant().Contains(query));
        }

        if (SelectedFilter != "All" && !string.IsNullOrEmpty(SelectedFilter))
        {
            filtered = filtered.Where(e =>
                e.FirstName.StartsWith(SelectedFilter, StringComparison.OrdinalIgnoreCase));
        }

        FilteredEmployees = new ObservableCollection<EmployeeCardViewModel>(filtered);
        HasNoResults = !FilteredEmployees.Any() && !IsLoading;
    }

    [RelayCommand]
    private void SelectFilter(string letter)
    {
        SelectedFilter = letter;
        foreach (var filter in AlphabetFilters) filter.IsSelected = filter.Letter == letter;
        ApplyFilters();
    }

    [RelayCommand]
    private async Task OpenEmployeeDetail(int employeeId)
    {
        await _navigationService.NavigateTo(ViewModelType.EmployeeDetails, employeeId);
    }

    #region Navigation
    [RelayCommand] private async Task NavigateToDashboard() => await _navigationService.NavigateTo(ViewModelType.HRDashboard);
    [RelayCommand] private async Task NavigateToProfile() => await _navigationService.NavigateTo(ViewModelType.Profile);
    [RelayCommand] private async Task NavigateToTimeOff() => await Task.CompletedTask;
    [RelayCommand] private async Task NavigateToAttendance() => await _navigationService.NavigateTo(ViewModelType.Attendance);
    [RelayCommand] private async Task NavigateToEmployees() => await Task.CompletedTask;
    [RelayCommand] private async Task NavigateToRecruitment() => await _navigationService.NavigateTo(ViewModelType.Recruitment);
    [RelayCommand] private async Task NavigateToForms() => await _navigationService.NavigateTo(ViewModelType.Forms);
    #endregion
}

public partial class EmployeeCardViewModel : ObservableObject
{
    [ObservableProperty] private int _id;
    [ObservableProperty] private string _firstName = string.Empty;
    [ObservableProperty] private string _lastName = string.Empty;
    [ObservableProperty] private string _jobTitle = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _discordUsername = string.Empty;
    [ObservableProperty] private string _department = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Initial => !string.IsNullOrEmpty(FirstName) ? FirstName[0].ToString().ToUpper() : "?";
}

public partial class AlphabetFilterItem : ObservableObject
{
    [ObservableProperty] private string _letter = string.Empty;
    [ObservableProperty] private bool _isSelected;
}