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

    #region User Profile Properties

    [ObservableProperty]
    private string _userName = "John Smith";

    [ObservableProperty]
    private string _userRole = "HR Manager";

    #endregion

    #region Search and Filter

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private string _selectedFilter = "All";

    [ObservableProperty]
    private ObservableCollection<AlphabetFilterItem> _alphabetFilters = new();

    #endregion

    #region Employee Data

    [ObservableProperty]
    private ObservableCollection<EmployeeCardViewModel> _allEmployees = new();

    [ObservableProperty]
    private ObservableCollection<EmployeeCardViewModel> _filteredEmployees = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasNoResults;

    #endregion

    public EmployeesViewModel(IEmployeeRepository employeeRepository, INavigationService navigationService)
    {
        _employeeRepository = employeeRepository;
        _navigationService = navigationService;
        InitializeAlphabetFilters();
    }

    // Parameterless constructor for design-time support
    public EmployeesViewModel() : this(null!, null!)
    {
        // Load mock data for design time
        LoadMockData();
    }

    private void InitializeAlphabetFilters()
    {
        AlphabetFilters.Add(new AlphabetFilterItem { Letter = "All", IsSelected = true });
        
        for (char c = 'A'; c <= 'Z'; c++)
        {
            AlphabetFilters.Add(new AlphabetFilterItem { Letter = c.ToString(), IsSelected = false });
        }
    }

    private void LoadMockData()
    {
        AllEmployees = new ObservableCollection<EmployeeCardViewModel>
        {
            new()
            {
                Id = 1,
                FirstName = "Alice",
                LastName = "Brown",
                JobTitle = "Senior Developer",
                Email = "alice.brown@company.com",
                DiscordUsername = "alice_b#1234",
                Department = "Engineering"
            },
            new()
            {
                Id = 2,
                FirstName = "Bob",
                LastName = "Smith",
                JobTitle = "Frontend Developer",
                Email = "bob.smith@company.com",
                DiscordUsername = "bob_dev#5678",
                Department = "Engineering"
            },
            new()
            {
                Id = 3,
                FirstName = "Carol",
                LastName = "Johnson",
                JobTitle = "UX Designer",
                Email = "carol.johnson@company.com",
                DiscordUsername = "carol_j#9012",
                Department = "Design"
            },
            new()
            {
                Id = 4,
                FirstName = "David",
                LastName = "Lee",
                JobTitle = "Product Manager",
                Email = "david.lee@company.com",
                DiscordUsername = "david_pm#3456",
                Department = "Product"
            },
            new()
            {
                Id = 5,
                FirstName = "Emma",
                LastName = "Wilson",
                JobTitle = "Backend Developer",
                Email = "emma.wilson@company.com",
                DiscordUsername = "emma_w#7890",
                Department = "Engineering"
            },
            new()
            {
                Id = 6,
                FirstName = "Frank",
                LastName = "Garcia",
                JobTitle = "DevOps Engineer",
                Email = "frank.garcia@company.com",
                DiscordUsername = "frank_g#2345",
                Department = "Engineering"
            },
            new()
            {
                Id = 7,
                FirstName = "Grace",
                LastName = "Martinez",
                JobTitle = "Data Analyst",
                Email = "grace.martinez@company.com",
                DiscordUsername = "grace_m#6789",
                Department = "Analytics"
            },
            new()
            {
                Id = 8,
                FirstName = "Henry",
                LastName = "Taylor",
                JobTitle = "QA Engineer",
                Email = "henry.taylor@company.com",
                DiscordUsername = "henry_t#1122",
                Department = "Quality"
            },
            new()
            {
                Id = 9,
                FirstName = "Isabella",
                LastName = "Anderson",
                JobTitle = "HR Specialist",
                Email = "isabella.anderson@company.com",
                DiscordUsername = "isabella_a#3344",
                Department = "Human Resources"
            },
            new()
            {
                Id = 10,
                FirstName = "James",
                LastName = "Thomas",
                JobTitle = "Security Engineer",
                Email = "james.thomas@company.com",
                DiscordUsername = "james_t#5566",
                Department = "Security"
            },
            new()
            {
                Id = 11,
                FirstName = "Karen",
                LastName = "White",
                JobTitle = "Marketing Manager",
                Email = "karen.white@company.com",
                DiscordUsername = "karen_w#7788",
                Department = "Marketing"
            },
            new()
            {
                Id = 12,
                FirstName = "Liam",
                LastName = "Harris",
                JobTitle = "Solutions Architect",
                Email = "liam.harris@company.com",
                DiscordUsername = "liam_h#9900",
                Department = "Engineering"
            },
            new()
            {
                Id = 13,
                FirstName = "Mia",
                LastName = "Clark",
                JobTitle = "Content Writer",
                Email = "mia.clark@company.com",
                DiscordUsername = "mia_c#1133",
                Department = "Marketing"
            },
            new()
            {
                Id = 14,
                FirstName = "Noah",
                LastName = "Lewis",
                JobTitle = "Mobile Developer",
                Email = "noah.lewis@company.com",
                DiscordUsername = "noah_l#2244",
                Department = "Engineering"
            },
            new()
            {
                Id = 15,
                FirstName = "Olivia",
                LastName = "Robinson",
                JobTitle = "Project Manager",
                Email = "olivia.robinson@company.com",
                DiscordUsername = "olivia_r#3355",
                Department = "Operations"
            },
            new()
            {
                Id = 16,
                FirstName = "Peter",
                LastName = "Walker",
                JobTitle = "Database Admin",
                Email = "peter.walker@company.com",
                DiscordUsername = "peter_w#4466",
                Department = "Engineering"
            },
            new()
            {
                Id = 17,
                FirstName = "Quinn",
                LastName = "Hall",
                JobTitle = "UI Designer",
                Email = "quinn.hall@company.com",
                DiscordUsername = "quinn_h#5577",
                Department = "Design"
            },
            new()
            {
                Id = 18,
                FirstName = "Rachel",
                LastName = "Young",
                JobTitle = "Business Analyst",
                Email = "rachel.young@company.com",
                DiscordUsername = "rachel_y#6688",
                Department = "Analytics"
            },
            new()
            {
                Id = 19,
                FirstName = "Samuel",
                LastName = "King",
                JobTitle = "Tech Lead",
                Email = "samuel.king@company.com",
                DiscordUsername = "samuel_k#7799",
                Department = "Engineering"
            },
            new()
            {
                Id = 20,
                FirstName = "Tina",
                LastName = "Wright",
                JobTitle = "Scrum Master",
                Email = "tina.wright@company.com",
                DiscordUsername = "tina_w#8800",
                Department = "Operations"
            }
        };

        ApplyFilters();
    }

    partial void OnSearchQueryChanged(string value)
    {
        ApplyFilters();
    }

    partial void OnSelectedFilterChanged(string value)
    {
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        var filtered = AllEmployees.AsEnumerable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var query = SearchQuery.ToLowerInvariant();
            filtered = filtered.Where(e =>
                e.FullName.ToLowerInvariant().Contains(query) ||
                e.Email.ToLowerInvariant().Contains(query) ||
                e.Department.ToLowerInvariant().Contains(query));
        }

        // Apply alphabetical filter
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

        // Update selection state
        foreach (var filter in AlphabetFilters)
        {
            filter.IsSelected = filter.Letter == letter;
        }

        // Force UI update
        var temp = new ObservableCollection<AlphabetFilterItem>(AlphabetFilters);
        AlphabetFilters = temp;

        ApplyFilters();
    }

    [RelayCommand]
    private async Task OpenEmployeeDetail(int employeeId)
    {
        await _navigationService.NavigateTo(ViewModelType.EmployeeDetails, employeeId);
    }

    public override async Task OnNavigatedToAsync()
    {
        if (IsLoading) return;
        
        // Always load mock data for now (no backend available)
        // In production, call LoadEmployeesAsync() instead
        LoadMockData();
        IsLoading = false;
        
        await base.OnNavigatedToAsync();
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task LoadEmployeesAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        HasNoResults = false;

        try
        {
            if (_employeeRepository == null)
            {
                LoadMockData();
                return;
            }
            
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
                            FirstName = e.BasicInfo?.FirstName ?? string.Empty,
                            LastName = e.BasicInfo?.LastName ?? string.Empty,
                            JobTitle = e.PositionDetails?.PositionName ?? "Unknown Position",
                            Email = e.ContactInfo?.ProjxonEmail ?? e.ContactInfo?.PersonalEmail ?? string.Empty,
                            DiscordUsername = "user#0000", // TODO: Add discord to DTO if needed
                            Department = "General" // TODO: Map from department ID
                        }));
                    ApplyFilters();
                });
            }
            else
            {
                ErrorMessage = result.ErrorMessage;
                // Load mock data as fallback
                LoadMockData();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            LoadMockData();
        }
        finally
        {
            IsLoading = false;
        }
    }

    #region Navigation Commands

    [RelayCommand]
    private async Task NavigateToDashboard()
    {
        await _navigationService.NavigateTo(ViewModelType.HRDashboard);
    }

    [RelayCommand]
    private async Task NavigateToProfile()
    {
        await _navigationService.NavigateTo(ViewModelType.Profile);
    }

    [RelayCommand]
    private async Task NavigateToTimeOff()
    {
        // TODO: Navigate to time off view when implemented
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task NavigateToAttendance()
    {
        await _navigationService.NavigateTo(ViewModelType.Attendance);
    }

    [RelayCommand]
    private async Task NavigateToEmployees()
    {
        // Already on employees page
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task NavigateToRecruitment()
    {
        await _navigationService.NavigateTo(ViewModelType.Recruitment);
    }

    #endregion
}

/// <summary>
/// Represents an employee card in the list view
/// </summary>
public partial class EmployeeCardViewModel : ObservableObject
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty]
    private string _firstName = string.Empty;

    [ObservableProperty]
    private string _lastName = string.Empty;

    [ObservableProperty]
    private string _jobTitle = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _discordUsername = string.Empty;

    [ObservableProperty]
    private string _department = string.Empty;

    public string FullName => $"{FirstName} {LastName}";
    public string Initial => !string.IsNullOrEmpty(FirstName) ? FirstName[0].ToString().ToUpper() : "?";
}

/// <summary>
/// Represents an alphabet filter button
/// </summary>
public partial class AlphabetFilterItem : ObservableObject
{
    [ObservableProperty]
    private string _letter = string.Empty;

    [ObservableProperty]
    private bool _isSelected;
}
