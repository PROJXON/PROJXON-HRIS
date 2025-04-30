using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSync.Modules.EmployeeManagement.Models;

public class Employee
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
    public string? JobTitle { get; set; }
    
    [Required]
    public int DepartmentId { get; set; }
    
    [ForeignKey("DepartmentId")]
    public Department? Department { get; set; }
    
    [Required]
    public int ManagerId { get; set; }
    
    [ForeignKey("ManagerId")]
    public Employee? Manager { get; set; }
    
    [Required]
    public int CoachId { get; set; }
    
    [ForeignKey("CoachId")]
    public Employee? Coach { get; set; }
    
    [StringLength(20)]
    public string? HierarchyLevel { get; set; }
    
    public DateTime? OnboardingDate { get; set; }
    
    public DateTime? OffboardingDate { get; set; }
    
    [StringLength(20)]
    public string? Status { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreateDateTime { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdateDateTime { get; set; }
    
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
    
    [StringLength(20)]
    public string? WorkAuthorizationType { get; set; }
    
    [Url]
    [StringLength(200)]
    public string? WorkAuthorizationDocumentUrl { get; set; }
    
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
    
    [Column(TypeName = "jsonb")]
    public List<string>? CanvasCoursesCompleted { get; set; }
    
    [Column(TypeName = "jsonb")]
    public List<string>? CanvasCertificates { get; set; }
    
    [StringLength(40)]
    public string? NewCompany { get; set; }
    
    [Column(TypeName = "jsonb")]
    public List<string>? OnboardingChecklist { get; set; }
}