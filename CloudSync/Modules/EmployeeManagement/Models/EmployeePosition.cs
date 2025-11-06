using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.EmployeeManagement.Enums;

namespace CloudSync.Modules.EmployeeManagement.Models;

public class EmployeePosition
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public Employee? Employee { get; set; }
    
    [EmailAddress][StringLength(60)] public string? ProjxonEmail { get; set; }
    public DateTime? OnboardingDate { get; set; }
    public DateTime? OffboardingDate { get; set; }
    public EmployeeStatus? EmploymentStatus { get; set; }
    public EmploymentType? EmploymentType { get; set; }
    [StringLength(30)] public string? EmployeeLifeCycleStage { get; set; }
    [StringLength(50)] public string? PositionName { get; set; }

    public int? ManagerId { get; set; }
    [ForeignKey("ManagerId")] public Employee? Manager { get; set; }
    
    public int? DepartmentId { get; set; }
    [ForeignKey("DepartmentId")] public Department? Department { get; set; }
    
    public int? SubDepartmentId { get; set; }
    [ForeignKey("SubDepartmentId")] public Department? SubDepartment { get; set; }

    public int? CoachId { get; set; }
    [ForeignKey("CoachId")] public Employee? Coach { get; set; }

    [StringLength(30)] public string? PreviousEmployers { get; set; }
    
    [StringLength(50)] public string? HierarchyLevel { get; set; }
    public DateTime? HireDate { get; set; }
    public DateTime? ExitDate { get; set; }
    [StringLength(30)] public string? RecruitingSource { get; set; }
}
