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
    
    [StringLength(40)]
    public string? LastName { get; set; }
    
    [EmailAddress]
    [StringLength(60)]
    public string? ProjxonEmail { get; set; }
    
    [Phone]
    [StringLength(15)]
    public string? Phone { get; set; }
    
    [Column(TypeName = "json")]
    public string? Address { get; set; }
    
    [EmailAddress]
    [StringLength(60)]
    public string? PersonalEmail { get; set; }
    
    [StringLength(40)]
    public string? EmergencyContactName { get; set; }
    
    [Phone]
    [StringLength(15)]
    public string? EmergencyContactPhone { get; set; }
    
    [StringLength(40)]
    public string? IdNumber { get; set; }
    
    [StringLength(40)]
    public string? IdType { get; set; }
    
    [StringLength(40)]
    public string? IdCountry { get; set; }
    
    [StringLength(40)]
    public string? IdState { get; set; }
    
    [StringLength(40)]
    public string? BirthCountry { get; set; }
    
    [StringLength(40)]
    public string? BirthState { get; set; }
    
    [StringLength(40)]
    public string? BirthCity { get; set; }
    
    public DateTime? BirthDate { get; set; }
    
    [StringLength(20)]
    public string? Gender { get; set; }
    
    [StringLength(20)]
    public string? PreferredPronouns { get; set; }
    
    [StringLength(20)]
    public string? PreferredName { get; set; }
    
    public int? PositionId { get; set; }
    [ForeignKey("PositionId")]
    public Position? Position { get; set; }
    
    public int? ManagerId { get; set; }
    
    [ForeignKey("ManagerId")]
    public Employee? Manager { get; set; }
    
    public int? CoachId { get; set; }
    
    [ForeignKey("CoachId")]
    public Employee? Coach { get; set; }
    
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
    public string? VisaNumber { get; set; }
    
    public DateTime? VisaExpirationDate { get; set; }
    
    [StringLength(20)]
    public string? WorkPermitNumber { get; set; }
    
    public DateTime? WorkExpirationDate { get; set; }
    
    [StringLength(20)]
    public string? EducationLevel { get; set; }
    
    [Column(TypeName = "jsonb")]
    public List<string>? UniversitiesAttended { get; set; }
    
    [Column(TypeName = "jsonb")]
    public List<string>? DegreesEarned { get; set; }
    
    [StringLength(30)]
    public string? TimeZone { get; set; }
    
    [StringLength(30)]
    public string? City { get; set; }
    
    [StringLength(30)]
    public string? State { get; set; }
    
    [StringLength(30)]
    public string? Country { get; set; }
    
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