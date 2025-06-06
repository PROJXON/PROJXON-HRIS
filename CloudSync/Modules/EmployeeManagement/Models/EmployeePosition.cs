using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Enums.UserManagement;

namespace CloudSync.Modules.EmployeeManagement.Models;

public class EmployeePosition
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public required Employee Employee { get; set; }

    
    [StringLength(50)]
    public string? PositionName { get; set; }
    
    public int? DepartmentId { get; set; }
    [ForeignKey("DepartmentId")]
    public Department? Department { get; set; }
    
    public int? SubDepartmentId { get; set; }
    [ForeignKey("SubDepartmentId")]
    public Department? SubDepartment { get; set; }
    
    [StringLength(50)]
    public string? HierarchyLevel { get; set; }
    
    public int? PositionId { get; set; }
    
    [ForeignKey("PositionId")]
    public EmployeePosition? PositionDetails { get; set; }
    
    public int? ManagerId { get; set; }
    
    [ForeignKey("ManagerId")]
    public Employee? Manager { get; set; }
    
    public int? CoachId { get; set; }
    
    [ForeignKey("CoachId")]
    public Employee? Coach { get; set; }
    
    public DateTime? OnboardingDate { get; set; }
    
    public DateTime? OffboardingDate { get; set; }
    
    [StringLength(30)]
    public string? EmploymentStatus { get; set; }
    
    [StringLength(30)]
    public string? EmploymentType { get; set; }
    
    [StringLength(30)]
    public string? RecruitingSource { get; set; }
}