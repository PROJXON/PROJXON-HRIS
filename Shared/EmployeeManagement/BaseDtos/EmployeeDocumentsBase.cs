namespace Shared.EmployeeManagement.BaseDtos;

public class EmployeeDocumentsBase
{
    public int Id { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? ResumeUrl { get; set; }
    public string? CoverLetterUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public string? PersonalWebsiteUrl { get; set; }
}
