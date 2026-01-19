using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Client.Services;
using Client.Utils.Classes;
using Client.Utils.Enums;
using Client.Utils.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Shared.EmployeeManagement.Requests;
using Shared.EmployeeManagement.Responses;
using Client.Models.EmployeeManagement;
using ServiceSessionArgs = Client.Services.SessionChangedEventArgs;

namespace Client.ViewModels;

public partial class ProfileViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly ISessionService _sessionService;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IFileService _fileService;
    private readonly IApiClient _apiClient;
    private readonly ILogger<ProfileViewModel>? _logger;
    
    // Hardcoded for dev environment
    private const string BaseUrl = "http://localhost:8080";

    public SidebarViewModel Sidebar { get; }

    #region State Properties

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isUploadingDocument;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _successMessage = string.Empty;

    [ObservableProperty]
    private bool _isBasicInformationTabSelected = true;

    [ObservableProperty]
    private bool _isAttachmentsTabSelected;

    #endregion

    #region Display Values

    [ObservableProperty] private string _firstName = string.Empty;
    [ObservableProperty] private string _lastName = string.Empty;
    [ObservableProperty] private string _preferredName = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _phoneNumber = string.Empty;
    [ObservableProperty] private string _department = string.Empty;
    [ObservableProperty] private string _jobTitle = string.Empty;
    [ObservableProperty] private string _employeeId = string.Empty;
    [ObservableProperty] private string _startDate = string.Empty;
    [ObservableProperty] private string _projxonEmail = string.Empty;
    [ObservableProperty] private string _discordUsername = string.Empty;

    #endregion

    #region Edit Values

    [ObservableProperty] private string _editFirstName = string.Empty;
    [ObservableProperty] private string _editLastName = string.Empty;
    [ObservableProperty] private string _editPreferredName = string.Empty;
    [ObservableProperty] private string _editEmail = string.Empty;
    [ObservableProperty] private string _editPhoneNumber = string.Empty;
    [ObservableProperty] private string _editJobTitle = string.Empty;
    [ObservableProperty] private string _editEmployeeId = string.Empty;
    [ObservableProperty] private string _editProjxonEmail = string.Empty;
    [ObservableProperty] private string _editDiscordUsername = string.Empty;
    
    // Avalonia DatePicker binds to DateTimeOffset?
    [ObservableProperty] private DateTimeOffset? _editStartDate; 

    #endregion

    #region Department Dropdown

    [ObservableProperty]
    private ObservableCollection<DepartmentOption> _departments = new();

    [ObservableProperty]
    private DepartmentOption? _selectedDepartment;

    #endregion

    #region Profile Sidebar / Header

    [ObservableProperty]
    private string _profileName = string.Empty;

    [ObservableProperty]
    private string _profileRole = string.Empty;

    [ObservableProperty]
    private string? _profilePictureUrl;

    #endregion

    #region Documents

    [ObservableProperty]
    private ObservableCollection<DocumentItem> _documents = new();

    // Critical: Backing fields to persist URLs during Save updates
    private string? _resumeUrl;
    private string? _coverLetterUrl;
    private string? _linkedInUrl;
    private string? _gitHubUrl;
    private string? _personalWebsiteUrl;

    #endregion

    private int _currentEmployeeId;

    public ProfileViewModel(
        INavigationService navigationService,
        ISessionService sessionService,
        IEmployeeRepository employeeRepository,
        IFileService fileService,
        IApiClient apiClient,
        SidebarViewModel sidebarViewModel,
        ILogger<ProfileViewModel>? logger = null)
    {
        _navigationService = navigationService;
        _sessionService = sessionService;
        _employeeRepository = employeeRepository;
        _fileService = fileService;
        _apiClient = apiClient;
        Sidebar = sidebarViewModel;
        _logger = logger;

        // Initialize immediately so binding context exists before data load
        InitializeDepartments();
    }

    // Constructor for Design-time
    public ProfileViewModel() : this(null!, null!, null!, null!, null!, null!) { }

    private void InitializeDepartments()
    {
        Departments = new ObservableCollection<DepartmentOption>
        {
            new DepartmentOption(1, "Business"),
            new DepartmentOption(2, "Human Resources"),
            new DepartmentOption(3, "Information Technology"),
            new DepartmentOption(4, "Marketing"),
            new DepartmentOption(5, "Operations"),
            new DepartmentOption(6, "Executive"),
        };
    }

    public override async Task OnNavigatedToAsync()
    {
        Sidebar.CurrentPage = "Profile";

        if (_sessionService.IsSessionValid)
        {
            // Always refresh data on navigation to ensure we have latest from DB
            await _sessionService.RefreshEmployeeDataAsync();
            if (_sessionService.CurrentEmployee != null)
            {
                LoadFromEmployee(_sessionService.CurrentEmployee);
            }
        }

        await base.OnNavigatedToAsync();
    }

    private void LoadFromEmployee(EmployeeResponse employee)
    {
        _currentEmployeeId = employee.Id ?? 0;

        // Basic Info
        FirstName = employee.BasicInfo?.FirstName ?? string.Empty;
        LastName = employee.BasicInfo?.LastName ?? string.Empty;
        PreferredName = employee.BasicInfo?.PreferredName ?? string.Empty;
        
        // Contact
        Email = employee.ContactInfo?.PersonalEmail ?? string.Empty;
        PhoneNumber = employee.ContactInfo?.PhoneNumber ?? string.Empty;
        ProjxonEmail = employee.ContactInfo?.ProjxonEmail ?? string.Empty;
        DiscordUsername = employee.ContactInfo?.DiscordUsername ?? string.Empty;

        // Position
        JobTitle = employee.PositionDetails?.PositionName ?? string.Empty;
        Department = employee.PositionDetails?.Department ?? string.Empty;

        // Set Selected Item for dropdown logic (even in display mode, we prep it)
        SelectedDepartment = Departments.FirstOrDefault(d => d.Name.Equals(Department, StringComparison.OrdinalIgnoreCase));

        // Start Date (Display)
        StartDate = employee.PositionDetails?.StartDate?.ToString("MM/dd/yyyy") ?? string.Empty;
        
        EmployeeId = $"EMP-{employee.Id:D4}";

        // Documents & Images
        ProfilePictureUrl = SanitizeServerUrl(employee.Documents?.ProfilePictureUrl);
        _resumeUrl = SanitizeServerUrl(employee.Documents?.ResumeUrl);
        _coverLetterUrl = SanitizeServerUrl(employee.Documents?.CoverLetterUrl);
        _linkedInUrl = employee.Documents?.LinkedInUrl;
        _gitHubUrl = employee.Documents?.GitHubUrl;
        _personalWebsiteUrl = employee.Documents?.PersonalWebsiteUrl;

        // Update Header/Sidebar info
        ProfileName = $"{FirstName} {LastName}".Trim();
        ProfileRole = string.IsNullOrWhiteSpace(JobTitle) ? "Employee" : JobTitle;

        BuildDocumentsList();
    }

    private string? SanitizeServerUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return null;
        if (url.Contains(":8080") && !url.Contains("localhost"))
        {
            var uri = new Uri(url);
            return $"{BaseUrl}{uri.PathAndQuery}";
        }
        if (url.StartsWith("/")) return $"{BaseUrl}{url}";
        return url;
    }

    private void BuildDocumentsList()
    {
        Documents.Clear();

        if (!string.IsNullOrEmpty(_resumeUrl))
        {
            Documents.Add(new DocumentItem
            {
                FileName = "Resume",
                UploadedDate = "Uploaded", 
                DocumentType = "resume",
                FileUrl = _resumeUrl
            });
        }
    }

    #region Tab Commands

    [RelayCommand]
    private void SelectBasicInformationTab()
    {
        IsBasicInformationTabSelected = true;
        IsAttachmentsTabSelected = false;
    }

    [RelayCommand]
    private void SelectAttachmentsTab()
    {
        IsBasicInformationTabSelected = false;
        IsAttachmentsTabSelected = true;
    }

    #endregion

    #region Edit Commands

    [RelayCommand]
    private void StartEditing()
    {
        // Copy current values to edit buffers
        EditFirstName = FirstName;
        EditLastName = LastName;
        EditPreferredName = PreferredName;
        EditEmail = Email;
        EditPhoneNumber = PhoneNumber;
        EditJobTitle = JobTitle;
        EditEmployeeId = EmployeeId; 
        EditProjxonEmail = ProjxonEmail;
        EditDiscordUsername = DiscordUsername;

        // Ensure dropdown is synced
        SelectedDepartment = Departments.FirstOrDefault(d => d.Name.Equals(Department, StringComparison.OrdinalIgnoreCase));
        
        // Parse Start Date string to DateTimeOffset for DatePicker
        if (DateTime.TryParse(StartDate, out var parsedDate))
        {
            EditStartDate = new DateTimeOffset(parsedDate);
        }
        else
        {
            EditStartDate = null;
        }

        IsEditing = true;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
    }

    [RelayCommand]
    private void CancelEditing()
    {
        IsEditing = false;
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private async Task SaveChangesAsync()
    {
        if (_currentEmployeeId == 0) return;

        IsLoading = true;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        try
        {
            // Convert DateTimeOffset to UTC DateTime for PostgreSQL compatibility
            DateTime? hireDateUtc = null;
            if (EditStartDate.HasValue)
            {
                // Use the date portion and specify it as UTC to satisfy PostgreSQL timestamptz
                hireDateUtc = DateTime.SpecifyKind(EditStartDate.Value.Date, DateTimeKind.Utc);
            }

            var updateRequest = new UpdateEmployeeRequest
            {
                Id = _currentEmployeeId,
                BasicInfo = new EmployeeBasicRequest
                {
                    FirstName = EditFirstName,
                    LastName = EditLastName,
                    PreferredName = EditPreferredName
                },
                ContactInfo = new EmployeeContactInfoRequest
                {
                    PersonalEmail = EditEmail,
                    PhoneNumber = EditPhoneNumber,
                    ProjxonEmail = EditProjxonEmail,
                    DiscordUsername = EditDiscordUsername
                },
                Documents = new EmployeeDocumentsRequest
                {
                    Id = 0,
                    ProfilePictureUrl = ProfilePictureUrl,
                    ResumeUrl = _resumeUrl,
                    CoverLetterUrl = _coverLetterUrl,
                    LinkedInUrl = _linkedInUrl,
                    GitHubUrl = _gitHubUrl,
                    PersonalWebsiteUrl = _personalWebsiteUrl
                },
                PositionDetails = new EmployeePositionRequest
                {
                    PositionName = EditJobTitle,
                    DepartmentId = SelectedDepartment?.Id,
                    HireDate = hireDateUtc  // Now using UTC DateTime
                }
            };

            var response = await _apiClient.PutAsync<EmployeeResponse>(
                $"api/Employee/{_currentEmployeeId}",
                updateRequest);

            if (response.IsSuccess)
            {
                // Force a session refresh
                await _sessionService.RefreshEmployeeDataAsync();
                
                // Reload local view
                if (_sessionService.CurrentEmployee != null)
                {
                    LoadFromEmployee(_sessionService.CurrentEmployee);
                }

                IsEditing = false;
                SuccessMessage = "Profile updated successfully!";
            }
            else
            {
                ErrorMessage = $"Failed to save: {response.ErrorMessage}";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
            _logger?.LogError(ex, "Error saving profile");
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion

    #region Upload Commands

    [RelayCommand]
    private async Task UploadResumeAsync()
    {
        if (_currentEmployeeId == 0) return;

        try
        {
            var file = await _fileService.PickDocumentAsync("Select Resume");
            if (file == null) return;

            IsUploadingDocument = true;
            ErrorMessage = string.Empty;

            var result = await _fileService.UploadFileAsync(file, _currentEmployeeId, "resume");

            if (result.Success && result.FileUrl != null)
            {
                _resumeUrl = SanitizeServerUrl(result.FileUrl);
                
                await _sessionService.RefreshEmployeeDataAsync();
                BuildDocumentsList();
                
                SuccessMessage = "Resume uploaded successfully!";
            }
            else
            {
                ErrorMessage = "Upload failed: " + (result.ErrorMessage ?? "Unknown error");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Upload error: {ex.Message}";
        }
        finally
        {
            IsUploadingDocument = false;
        }
    }

    [RelayCommand]
    private void ViewDocument(DocumentItem? doc)
    {
        if (doc?.FileUrl != null)
        {
            _fileService.OpenUrl(doc.FileUrl);
        }
    }

    #endregion
}

// ---------------------------------------------------------
// Helper Classes
// ---------------------------------------------------------

public class DocumentItem
{
    public string FileName { get; set; } = string.Empty;
    public string UploadedDate { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
}

public class DepartmentOption(int id, string name)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public override string ToString() => Name;
}