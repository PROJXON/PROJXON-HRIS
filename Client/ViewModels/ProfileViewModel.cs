using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Client.Services;
using Client.Utils.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Client.ViewModels;

/// <summary>
/// ViewModel for the Profile view
/// Allows users to view and edit their personal information and documents
/// </summary>
public partial class ProfileViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    #region Edit Mode State

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    #endregion

    #region Tab Selection

    [ObservableProperty]
    private bool _isBasicInformationTabSelected = true;

    [ObservableProperty]
    private bool _isAttachmentsTabSelected;

    #endregion

    #region Personal Details - Display Values

    [ObservableProperty]
    private string _firstName = string.Empty;

    [ObservableProperty]
    private string _lastName = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _phoneNumber = string.Empty;

    [ObservableProperty]
    private string _department = string.Empty;

    [ObservableProperty]
    private string _jobTitle = string.Empty;

    [ObservableProperty]
    private string _employeeId = string.Empty;

    [ObservableProperty]
    private string _startDate = string.Empty;

    [ObservableProperty]
    private string _discordUsername = string.Empty;

    #endregion

    #region Personal Details - Edit Values (backing fields for editing)

    [ObservableProperty]
    private string _editFirstName = string.Empty;

    [ObservableProperty]
    private string _editLastName = string.Empty;

    [ObservableProperty]
    private string _editEmail = string.Empty;

    [ObservableProperty]
    private string _editPhoneNumber = string.Empty;

    [ObservableProperty]
    private string _editDepartment = string.Empty;

    [ObservableProperty]
    private string _editJobTitle = string.Empty;

    [ObservableProperty]
    private string _editEmployeeId = string.Empty;

    [ObservableProperty]
    private string _editStartDate = string.Empty;

    [ObservableProperty]
    private string _editDiscordUsername = string.Empty;

    #endregion

    #region Sidebar Profile Info

    [ObservableProperty]
    private string _profileName = string.Empty;

    [ObservableProperty]
    private string _profileRole = string.Empty;

    #endregion

    #region Documents/Attachments

    [ObservableProperty]
    private ObservableCollection<DocumentItem> _documents = new();

    #endregion

    public ProfileViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        LoadMockData();
    }

    // Parameterless constructor for design-time support
    public ProfileViewModel() : this(null!)
    {
    }

    private void LoadMockData()
    {
        // Mock data matching the Figma design
        FirstName = "John";
        LastName = "Anderson";
        Email = "john.anderson@company.com";
        PhoneNumber = "(555) 123-4567";
        Department = "Human Resources";
        JobTitle = "HR Manager";
        EmployeeId = "HR-001";
        StartDate = "01/15/2020";
        DiscordUsername = "john_hr#1234";

        // Sidebar info
        ProfileName = "John Smith";
        ProfileRole = "HR Manager";

        // Documents
        Documents = new ObservableCollection<DocumentItem>
        {
            new DocumentItem
            {
                FileName = "Employment_Contract.pdf",
                UploadedDate = "Uploaded on 2020-01-15"
            },
            new DocumentItem
            {
                FileName = "ID_Document.pdf",
                UploadedDate = "Uploaded on 2020-01-15"
            },
            new DocumentItem
            {
                FileName = "Certifications.pdf",
                UploadedDate = "Uploaded on 2022-03-10"
            }
        };
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

    #region Edit Mode Commands

    [RelayCommand]
    private void StartEditing()
    {
        // Copy current values to edit fields
        EditFirstName = FirstName;
        EditLastName = LastName;
        EditEmail = Email;
        EditPhoneNumber = PhoneNumber;
        EditDepartment = Department;
        EditJobTitle = JobTitle;
        EditEmployeeId = EmployeeId;
        EditStartDate = StartDate;
        EditDiscordUsername = DiscordUsername;

        IsEditing = true;
    }

    [RelayCommand]
    private void CancelEditing()
    {
        // Discard changes and exit edit mode
        IsEditing = false;
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private async Task SaveChangesAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            // TODO: Call API to save changes
            // For now, just update local values
            await Task.Delay(500); // Simulate API call

            FirstName = EditFirstName;
            LastName = EditLastName;
            Email = EditEmail;
            PhoneNumber = EditPhoneNumber;
            Department = EditDepartment;
            JobTitle = EditJobTitle;
            EmployeeId = EditEmployeeId;
            StartDate = EditStartDate;
            DiscordUsername = EditDiscordUsername;

            // Update sidebar name if first/last name changed
            ProfileName = $"{FirstName} {LastName}";

            IsEditing = false;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to save changes: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion

    #region Document Commands

    [RelayCommand]
    private async Task UploadDocumentAsync()
    {
        // TODO: Implement file picker and upload
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task ViewDocumentAsync(DocumentItem? document)
    {
        if (document == null) return;
        
        // TODO: Implement document viewing
        await Task.CompletedTask;
    }

    #endregion

    #region Navigation Commands

    [RelayCommand]
    private async Task NavigateToDashboard()
    {
        await _navigationService.NavigateTo(ViewModelType.HRDashboard);
    }

    [RelayCommand]
    private async Task NavigateToProfile()
    {
        // Already on profile, no action needed
        await Task.CompletedTask;
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
        await _navigationService.NavigateTo(ViewModelType.Employees);
    }

    [RelayCommand]
    private async Task NavigateToRecruitment()
    {
        await _navigationService.NavigateTo(ViewModelType.Recruitment);
    }
    
    [RelayCommand]
    private async Task NavigateToForms()
    {
        await _navigationService.NavigateTo(ViewModelType.Forms);
    }

    #endregion

    public override async Task OnNavigatedToAsync()
    {
        // TODO: Load actual profile data from API
        await base.OnNavigatedToAsync();
    }
}

/// <summary>
/// Model for document/attachment items
/// </summary>
public class DocumentItem
{
    public string FileName { get; set; } = string.Empty;
    public string UploadedDate { get; set; } = string.Empty;
}
