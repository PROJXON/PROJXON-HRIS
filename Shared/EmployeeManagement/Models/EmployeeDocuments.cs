using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.EmployeeManagement.Models;

public class EmployeeDocuments
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public Employee Employee { get; set; }
    
    [Url]
    [StringLength(200)]
    public string? ProfilePictureUrl { get; set; }
    
    [Url]
    [StringLength(200)]
    public string? ResumeUrl { get; set; }
    
    [Url]
    [StringLength(200)]
    public string? CoverLetterUrl { get; set; }
    
    [Url]
    [StringLength(200)]
    public string? LinkedInUrl { get; set; }
    
    [Url]
    [StringLength(200)]
    public string? GitHubUrl { get; set; }
    
    [Url]
    [StringLength(200)]
    public string? PersonalWebsiteUrl { get; set; }
}