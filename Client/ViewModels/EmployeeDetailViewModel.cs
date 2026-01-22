using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Client.Models.EmployeeManagement;
using Client.Services;
using Client.Utils.Enums;
using Client.Utils.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Attendance;
using Shared.EmployeeManagement.Responses;

namespace Client.ViewModels;

public partial class EmployeeDetailViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IEmployeeRepository? _employeeRepository;
    private readonly IApiClient _apiClient;
    private readonly IFileService _fileService;
    private int _currentEmployeeId;
    
    private const string BaseUrl = "http://localhost:8080";

    public SidebarViewModel Sidebar { get; }

    #region Tab Selection

    [ObservableProperty]
    private bool _isPersonalInfoTabSelected = true;

    [ObservableProperty]
    private bool _isAttendanceTabSelected;
    
    [ObservableProperty]
    private bool _isDocumentsTabSelected;

    #endregion

    #region Employee Information

    [ObservableProperty]
    private string _employeeFullName = string.Empty;

    [ObservableProperty]
    private string _employeeInitial = string.Empty;

    [ObservableProperty]
    private string _employeeJobTitle = string.Empty;

    [ObservableProperty]
    private string _employeeEmail = string.Empty;

    [ObservableProperty]
    private string _employeeDiscord = string.Empty;

    [ObservableProperty]
    private string _employeeDepartment = string.Empty;

    [ObservableProperty]
    private string _employeeId = string.Empty;

    [ObservableProperty]
    private string _employeePhone = string.Empty;

    [ObservableProperty]
    private string _employeeStartDate = string.Empty;

    [ObservableProperty]
    private string _employeeLocation = string.Empty;

    [ObservableProperty]
    private string _employeeType = string.Empty;

    #endregion

    #region Attendance Pagination

    // Full list (cache)
    private ObservableCollection<AttendanceRecordViewModel> _allAttendanceRecords = new();

    // Display list
    [ObservableProperty]
    private ObservableCollection<AttendanceRecordViewModel> _pagedAttendanceRecords = new();

    [ObservableProperty]
    private int _attendanceCurrentPage = 1;

    private const int AttendancePageSize = 5;

    public int AttendanceTotalPages => (int)Math.Ceiling((double)_allAttendanceRecords.Count / AttendancePageSize);
    public bool CanNavigateAttendanceNext => AttendanceCurrentPage < AttendanceTotalPages;
    public bool CanNavigateAttendancePrev => AttendanceCurrentPage > 1;

    [ObservableProperty]
    private bool _hasNoAttendanceRecords;

    #endregion

    #region Documents Pagination

    // Full list (cache)
    [ObservableProperty]
    private ObservableCollection<DocumentItem> _documents = new();
    
    // Display list
    [ObservableProperty]
    private ObservableCollection<DocumentItem> _pagedDocuments = new();
    
    [ObservableProperty]
    private int _currentPage = 1;
    
    private const int PageSize = 5;

    public int TotalPages => (int)Math.Ceiling((double)Documents.Count / PageSize);
    public bool CanNavigateNext => CurrentPage < TotalPages;
    public bool CanNavigatePrev => CurrentPage > 1;
    
    [ObservableProperty]
    private bool _hasNoDocuments;

    #endregion

    #region Loading State

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    #endregion

    public EmployeeDetailViewModel(
        INavigationService navigationService,
        SidebarViewModel sidebarViewModel,
        IEmployeeRepository employeeRepository,
        IApiClient apiClient,
        IFileService fileService)
    {
        _navigationService = navigationService;
        Sidebar = sidebarViewModel;
        _employeeRepository = employeeRepository;
        _apiClient = apiClient;
        _fileService = fileService;
    }

    // Parameterless constructor for design-time support
    public EmployeeDetailViewModel() : this(null!, new SidebarViewModel(), null!, null!, null!)
    {
    }

    public void SetEmployeeId(int employeeId)
    {
        _currentEmployeeId = employeeId;
    }

    public override async Task OnNavigatedToAsync()
    {
        Sidebar.CurrentPage = "Employees";
        Sidebar.SetPortalMode(false);
        
        if (_currentEmployeeId > 0 && _employeeRepository != null)
        {
            await LoadEmployeeAsync();
            await LoadAttendanceAsync();
            await LoadDocumentsAsync(_currentEmployeeId);
        }

        await base.OnNavigatedToAsync();
    }

    private async Task LoadEmployeeAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var result = await _employeeRepository!.GetByIdAsync(_currentEmployeeId);
            if (result.IsSuccess && result.Value != null)
            {
                var emp = result.Value;

                // Bind Header Info
                EmployeeFullName = $"{emp.BasicInfo?.FirstName} {emp.BasicInfo?.LastName}".Trim();
                EmployeeInitial = !string.IsNullOrEmpty(emp.BasicInfo?.FirstName) 
                    ? emp.BasicInfo.FirstName[0].ToString().ToUpper() 
                    : "?";
                EmployeeJobTitle = emp.PositionDetails?.PositionName ?? "N/A";
                EmployeeEmail = emp.ContactInfo?.ProjxonEmail ?? emp.ContactInfo?.PersonalEmail ?? "N/A";
                
                // Discord Binding
                EmployeeDiscord = !string.IsNullOrWhiteSpace(emp.ContactInfo?.DiscordUsername) 
                    ? emp.ContactInfo.DiscordUsername 
                    : "N/A";

                // Department Binding
                EmployeeDepartment = emp.PositionDetails?.Department ?? "General";

                EmployeeId = $"EMP-{emp.Id:D4}";
                EmployeePhone = emp.ContactInfo?.PhoneNumber ?? "N/A";
                EmployeeStartDate = emp.PositionDetails?.HireDate?.ToString("MMMM dd, yyyy") ?? "N/A";
                
                // Location
                var addr = emp.ContactInfo?.PermanentAddress;
                EmployeeLocation = addr != null ? GetLocationString(addr) : "N/A";

                EmployeeType = emp.PositionDetails?.EmploymentType?.ToString() ?? "N/A";
            }
            else
            {
                ErrorMessage = result.ErrorMessage;
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

    private async Task LoadAttendanceAsync()
    {
        try
        {
            var result = await _apiClient.GetAllAsync<IEnumerable<AttendanceResponse>>($"api/Attendance/{_currentEmployeeId}");
            
            _allAttendanceRecords.Clear();
            
            if (result.IsSuccess && result.Data != null)
            {
                var sorted = result.Data.OrderByDescending(x => x.Date).ToList();
                foreach (var rec in sorted)
                {
                    _allAttendanceRecords.Add(new AttendanceRecordViewModel
                    {
                        Date = rec.Date,
                        StartTime = rec.StartTime,
                        EndTime = rec.EndTime
                    });
                }
            }
            
            HasNoAttendanceRecords = _allAttendanceRecords.Count == 0;
            
            // Initialize Pagination
            AttendanceCurrentPage = 1;
            UpdatePagedAttendance();
        }
        catch
        {
            HasNoAttendanceRecords = true;
        }
    }

    private void UpdatePagedAttendance()
    {
        var items = _allAttendanceRecords.Skip((AttendanceCurrentPage - 1) * AttendancePageSize).Take(AttendancePageSize);
        PagedAttendanceRecords = new ObservableCollection<AttendanceRecordViewModel>(items);

        OnPropertyChanged(nameof(AttendanceTotalPages));
        OnPropertyChanged(nameof(CanNavigateAttendanceNext));
        OnPropertyChanged(nameof(CanNavigateAttendancePrev));
    }

    private async Task LoadDocumentsAsync(int employeeId)
    {
        try 
        {
            Documents.Clear();

            if (_employeeRepository != null)
            {
                var empResult = await _employeeRepository.GetByIdAsync(employeeId);
                if (empResult.IsSuccess && empResult.Value != null)
                {
                    var emp = empResult.Value;
                    
                    if (!string.IsNullOrEmpty(emp.Documents?.ResumeUrl))
                    {
                        Documents.Add(new DocumentItem 
                        {
                            Id = -1,
                            FileName = "Resume",
                            UploadedDate = "On File",
                            DocumentType = "resume",
                            FileUrl = SanitizeUrl(emp.Documents.ResumeUrl)
                        });
                    }
                    
                    if (!string.IsNullOrEmpty(emp.Documents?.CoverLetterUrl))
                    {
                        Documents.Add(new DocumentItem 
                        {
                            Id = -2,
                            FileName = "Cover Letter",
                            UploadedDate = "On File",
                            DocumentType = "cover-letter",
                            FileUrl = SanitizeUrl(emp.Documents.CoverLetterUrl)
                        });
                    }
                    
                    if (!string.IsNullOrEmpty(emp.Documents?.ProfilePictureUrl))
                    {
                        Documents.Add(new DocumentItem 
                        {
                            Id = -3,
                            FileName = "Profile Picture",
                            UploadedDate = "On File",
                            DocumentType = "profile-picture",
                            FileUrl = SanitizeUrl(emp.Documents.ProfilePictureUrl)
                        });
                    }
                }
            }

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
                        FileUrl = SanitizeUrl(file.FileUrl)
                    });
                }
            }
            
            CurrentPage = 1;
            UpdatePagedDocuments(); 
            HasNoDocuments = Documents.Count == 0;
        }
        catch
        {
            HasNoDocuments = true;
        }
    }

    private void UpdatePagedDocuments()
    {
        var items = Documents.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
        PagedDocuments = new ObservableCollection<DocumentItem>(items);
        
        OnPropertyChanged(nameof(TotalPages));
        OnPropertyChanged(nameof(CanNavigateNext));
        OnPropertyChanged(nameof(CanNavigatePrev));
    }
    
    private string? SanitizeUrl(string? url)
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

    private static string GetLocationString(Shared.EmployeeManagement.Responses.AddressResponse? address)
    {
        if (address == null) return "N/A";

        var parts = new[]
        {
            address.City,
            address.StateOrProvince
        };

        var location = string.Join(", ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        return string.IsNullOrWhiteSpace(location) ? "N/A" : location;
    }

    #region Tab Commands

    [RelayCommand]
    private void SelectPersonalInfoTab()
    {
        IsPersonalInfoTabSelected = true;
        IsAttendanceTabSelected = false;
        IsDocumentsTabSelected = false;
    }

    [RelayCommand]
    private void SelectAttendanceTab()
    {
        IsPersonalInfoTabSelected = false;
        IsAttendanceTabSelected = true;
        IsDocumentsTabSelected = false;
    }

    [RelayCommand]
    private void SelectDocumentsTab()
    {
        IsPersonalInfoTabSelected = false;
        IsAttendanceTabSelected = false;
        IsDocumentsTabSelected = true;
    }

    #endregion

    #region Pagination Commands (Documents)

    [RelayCommand]
    private void NextPage()
    {
        if (CanNavigateNext)
        {
            CurrentPage++;
            UpdatePagedDocuments();
        }
    }

    [RelayCommand]
    private void PreviousPage()
    {
        if (CanNavigatePrev)
        {
            CurrentPage--;
            UpdatePagedDocuments();
        }
    }

    #endregion

    #region Pagination Commands (Attendance)

    [RelayCommand] 
    private void NextAttendancePage() 
    { 
        if (CanNavigateAttendanceNext) 
        { 
            AttendanceCurrentPage++; 
            UpdatePagedAttendance(); 
        } 
    }

    [RelayCommand] 
    private void PreviousAttendancePage() 
    { 
        if (CanNavigateAttendancePrev) 
        { 
            AttendanceCurrentPage--; 
            UpdatePagedAttendance(); 
        } 
    }

    #endregion

    #region Feature Commands

    [RelayCommand]
    private void RequestChange()
    {
        Sidebar.TriggerComingSoonCommand.Execute(null);
    }

    [RelayCommand]
    private void SelectPerformanceTab()
    {
        Sidebar.TriggerComingSoonCommand.Execute(null);
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await _navigationService.NavigateTo(ViewModelType.Employees);
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

    #region Helper Classes

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

    #endregion
}

/// <summary>
/// Represents an attendance record entry
/// </summary>
public partial class AttendanceRecordViewModel : ObservableObject
{
    [ObservableProperty]
    private DateTime _date;

    [ObservableProperty]
    private TimeSpan _startTime;

    [ObservableProperty]
    private TimeSpan _endTime;

    public string DateDisplay => Date.ToString("dddd, MMMM d, yyyy");

    public string TimeRange => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";

    public string TotalHours
    {
        get
        {
            var duration = EndTime - StartTime;
            return $"{duration.TotalHours:F1} hrs";
        }
    }
}