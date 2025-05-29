using Shared.EmployeeManagement.Enums;
using Shared.Enums.UserManagement;

namespace Shared.EmployeeManagement.Responses;

public class EmployeeResponse
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProjxonEmail { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? PersonalEmail { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? BirthCountry { get; set; }
    public string? BirthState { get; set; }
    public string? BirthCity { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Gender { get; set; }
    public string? PreferredPronouns { get; set; }
    public string? PreferredName { get; set; }
    public int? ManagerId { get; set; }
    public int? CoachId { get; set; }
    public DateTime? OnboardingDate { get; set; }
    public DateTime? OffboardingDate { get; set; }
    public EmployeeStatus? Status { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? ResumeUrl { get; set; }
    public string? CoverLetterUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public string? PersonalWebsiteUrl { get; set; }
    public string? EducationLevel { get; set; }
    public List<string>? UniversitiesAttended { get; set; }
    public List<string>? DegreesEarned { get; set; }
    public string? TimeZone { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? RecruitingSource { get; set; }
    public List<string>? CanvasCoursesCompleted { get; set; }
    public List<string>? CanvasCertificates { get; set; }
    public string? NewCompany { get; set; }
    public List<string>? OnboardingChecklist { get; set; }
}