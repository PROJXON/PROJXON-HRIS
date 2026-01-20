using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CloudSync.Modules.EmployeeManagement.Models;

namespace CloudSync.Modules.CandidateManagement.Models;

public class Candidate
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [StringLength(40)]
    public string? FirstName { get; set; }
    
    [Required]
    [StringLength(40)]
    public string? LastName { get; set; }
    
    [EmailAddress]
    [StringLength(40)]
    public string? Email { get; set; }
    
    [Phone]
    [StringLength(15)]
    public string? Phone { get; set; }
    
    [Required]
    [StringLength(40)]
    public string? JobAppliedFor { get; set; }
    
    public DateTime? AvailabilityDate { get; set; }
        
    public DateTime? OnboardingDate { get; set; }
    
    [StringLength(20)]
    public string? Status { get; set; }

    // FIXED: Removed [DatabaseGenerated] so EF Core sends the value we set in code
    public DateTime CreateDateTime { get; set; } = DateTime.UtcNow;
    
    // FIXED: Removed [DatabaseGenerated]
    public DateTime UpdateDateTime { get; set; } = DateTime.UtcNow;
    
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
    
    [StringLength(20)]
    public string? WorkAuthorizationType { get; set; }
    
    [StringLength(20)]
    public string? EducationLevel { get; set; }
    
    [Column(TypeName = "jsonb")]
    public List<string>? UniversitiesAttended { get; set; }
    
    [StringLength(30)]
    public string? TimeZone { get; set; }
    
    [StringLength(30)]
    public string? City { get; set; }
    
    [StringLength(30)]
    public string? State { get; set; }
    
    [StringLength(30)]
    public string? RecruitingSource { get; set; }
    
    public int? InterviewerId { get; set; }
    
    [ForeignKey("InterviewerId")]
    public Employee? Interviewer { get; set; }
    
    [Column(TypeName = "jsonb")]
    public List<Employee>? Interviewers { get; set; }
    
    public DateTime? InterviewDateTime { get; set; }
    
    [StringLength(400)]
    public string? Notes { get; set; }
}