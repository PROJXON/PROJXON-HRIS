using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Client.Services;
using Client.Utils.Classes;
using Client.Utils.Enums;
using Client.Utils.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
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
    private readonly IUserPreferencesService _userPreferencesService;
    private readonly ILogger<ProfileViewModel>? _logger;
    private readonly string _baseUrl;

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

    [ObservableProperty]
    private ObservableCollection<DocumentItem> _pagedDocuments = new();

    [ObservableProperty]
    private int _currentPage = 1;

    private const int PageSize = 5;

    public int TotalPages => (int)Math.Ceiling((double)Documents.Count / PageSize);
    public bool CanNavigateNext => CurrentPage < TotalPages;
    public bool CanNavigatePrev => CurrentPage > 1;

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
        IUserPreferencesService userPreferencesService,
        IConfiguration configuration,
        ILogger<ProfileViewModel>? logger = null)
    {
        _navigationService = navigationService;
        _sessionService = sessionService;
        _employeeRepository = employeeRepository;
        _fileService = fileService;
        _apiClient = apiClient;
        Sidebar = sidebarViewModel;
        _userPreferencesService = userPreferencesService;
        _logger = logger;
        _baseUrl = configuration["CloudSyncUrl"] ?? "http://localhost:8080";

        // Initialize immediately so binding context exists before data load
        InitializeDepartments();
    }

    // Constructor for Design-time
    public ProfileViewModel() : this(null!, null!, null!, null!, null!, null!, null!, null!) { }

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
        
        // Determine portal mode from saved preference
        var portalPref = await _userPreferencesService.GetPortalPreferenceAsync();
        Sidebar.SetPortalMode(portalPref == PortalType.Intern);

        if (_sessionService.IsSessionValid)
        {
            // Always refresh data on navigation to ensure we have latest from DB
            await _sessionService.RefreshEmployeeDataAsync();
            if (_sessionService.CurrentEmployee != null)
            {
                LoadFromEmployee(_sessionService.CurrentEmployee); // Loads basic info
                await LoadDocumentsAsync(_sessionService.CurrentEmployee.Id ?? 0); // Loads dynamic file list
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
    }

    private string? SanitizeServerUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return null;

        if (url.Contains("storage.googleapis.com"))
        {
            const string bucketName = "projxon-hris-uploads"; 
        
            var parts = url.Split(new[] { bucketName }, StringSplitOptions.None);
            if (parts.Length > 1)
            {
                var objectPath = parts[1].TrimStart('/');
                var encodedPath = System.Net.WebUtility.UrlEncode(objectPath);
            
                return $"{_baseUrl.TrimEnd('/')}/api/Document/view/{encodedPath}";
            }
        }

        if (url.StartsWith("http")) return url;
    
        var baseUri = _baseUrl.TrimEnd('/');
        var path = url.StartsWith("/") ? url : "/" + url;
    
        return $"{baseUri}{path}";
    }

    private async Task LoadDocumentsAsync(int employeeId)
    {
        try 
        {
            Documents.Clear();
            
            if (!string.IsNullOrEmpty(_resumeUrl))
            {
                Documents.Add(new DocumentItem 
                {
                    Id = -1, 
                    FileName = "Resume",
                    UploadedDate = "On File",
                    DocumentType = "resume",
                    FileUrl = _resumeUrl
                });
            }

            if (!string.IsNullOrEmpty(_coverLetterUrl))
            {
                Documents.Add(new DocumentItem 
                {
                    Id = -2,
                    FileName = "Cover Letter",
                    UploadedDate = "On File",
                    DocumentType = "cover-letter",
                    FileUrl = _coverLetterUrl
                });
            }
            
            if (!string.IsNullOrEmpty(ProfilePictureUrl))
            {
                Documents.Add(new DocumentItem 
                {
                    Id = -3,
                    FileName = "Profile Picture",
                    UploadedDate = "On File",
                    DocumentType = "profile-picture",
                    FileUrl = ProfilePictureUrl
                });
            }

            // Fetch Generic Files from API
            var result = await _apiClient.GetAllAsync<IEnumerable<EmployeeFileResponse>>($"api/Document/files/{employeeId}");
            
            if (result.IsSuccess && result.Data != null)
            {
                foreach(var file in result.Data)
                {
                    Documents.Add(new DocumentItem 
                    {
                        Id = file.Id,
                        FileName = file.FileName,
                        UploadedDate = file.UploadedAt.ToLocalTime().ToString("MM/dd/yyyy"),
                        DocumentType = file.Category,
                        FileUrl = SanitizeServerUrl(file.FileUrl)
                    });
                }
            }
            
            // Update Pagination
            CurrentPage = 1; 
            BuildDocumentsList(); 
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to load documents list");
        }
    }

    private void BuildDocumentsList()
    {
        UpdatePagedDocuments();
        OnPropertyChanged(nameof(TotalPages));
        OnPropertyChanged(nameof(CanNavigateNext));
        OnPropertyChanged(nameof(CanNavigatePrev));
    }

    private void UpdatePagedDocuments()
    {
        var items = Documents.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
        PagedDocuments = new ObservableCollection<DocumentItem>(items);
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
        EditFirstName = FirstName;
        EditLastName = LastName;
        EditPreferredName = PreferredName;
        EditEmail = Email;
        EditPhoneNumber = PhoneNumber;
        EditJobTitle = JobTitle;
        EditEmployeeId = EmployeeId; 
        EditProjxonEmail = ProjxonEmail;
        EditDiscordUsername = DiscordUsername;

        SelectedDepartment = Departments.FirstOrDefault(d => d.Name.Equals(Department, StringComparison.OrdinalIgnoreCase));
        
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
            DateTime? hireDateUtc = null;
            if (EditStartDate.HasValue)
            {
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
                    HireDate = hireDateUtc
                }
            };

            var response = await _apiClient.PutAsync<EmployeeResponse>(
                $"api/Employee/{_currentEmployeeId}",
                updateRequest);

            if (response.IsSuccess)
            {
                await _sessionService.RefreshEmployeeDataAsync();
                
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

    #region Pagination Commands

    [RelayCommand]
    private void NextPage()
    {
        if (CanNavigateNext)
        {
            CurrentPage++;
            UpdatePagedDocuments();
            OnPropertyChanged(nameof(CanNavigateNext));
            OnPropertyChanged(nameof(CanNavigatePrev));
        }
    }

    [RelayCommand]
    private void PreviousPage()
    {
        if (CanNavigatePrev)
        {
            CurrentPage--;
            UpdatePagedDocuments();
            OnPropertyChanged(nameof(CanNavigateNext));
            OnPropertyChanged(nameof(CanNavigatePrev));
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
                await LoadDocumentsAsync(_currentEmployeeId);
                
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
    private async Task UploadDocumentAsync()
    {
        if (_currentEmployeeId == 0) return;

        try
        {
            var file = await _fileService.PickFileAsync("Select Document", new[] { 
                ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".csv", 
                ".ppt", ".pptx", ".txt", ".rtf" 
            });
            if (file == null) return;

            IsUploadingDocument = true;
            ErrorMessage = string.Empty;

            var result = await _fileService.UploadFileAsync(file, _currentEmployeeId, "other");

            if (result.Success && result.FileUrl != null)
            {
                await LoadDocumentsAsync(_currentEmployeeId);
                SuccessMessage = "Document uploaded successfully!";
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
    
    // Delete Document Command
    [RelayCommand]
    private async Task DeleteDocument(DocumentItem? doc)
    {
        if (doc == null || _currentEmployeeId == 0) return;
        
        IsLoading = true;
        ErrorMessage = string.Empty;
        
        try 
        {
            ApiResponse<object?> response;
            
            // Check if it is a specific file (Resume, etc) or generic
            if (doc.Id < 0)
            {
                var endpoint = $"api/Document/{_currentEmployeeId}/{doc.DocumentType}";
                response = await _apiClient.DeleteAsync<object>(endpoint);
                
                // If successful, clear the local URL variable so it doesn't reappear
                if (response.IsSuccess)
                {
                    if (doc.DocumentType == "resume") _resumeUrl = null;
                    else if (doc.DocumentType == "cover-letter") _coverLetterUrl = null;
                    else if (doc.DocumentType == "profile-picture") ProfilePictureUrl = null;
                    
                    // Sync session
                    await _sessionService.RefreshEmployeeDataAsync();
                }
            }
            else
            {
                response = await _apiClient.DeleteAsync<object>("api/Document/file", doc.Id);
            }
            
            if (response.IsSuccess)
            {
                SuccessMessage = "File deleted successfully.";
                await LoadDocumentsAsync(_currentEmployeeId);
            }
            else 
            {
                ErrorMessage = "Failed to delete: " + response.ErrorMessage;
            }
        }
        catch (Exception ex)
        {
             ErrorMessage = "Delete error: " + ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion
}

public class DocumentItem
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string UploadedDate { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
}

public class EmployeeFileResponse
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string Category { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
}

public class DepartmentOption(int id, string name)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public override string ToString() => Name;
}