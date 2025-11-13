using Shared.EmployeeManagement.Enums;
using Shared.EmployeeManagement.Responses;

namespace Shared.EmployeeManagement.BaseDtos;

public class EmployeePositionBase
{
    public int Id { get; set; }
    public string? ProjxonEmail { get; set; }
    public DateTime? OnboardingDate { get; set; }
    public DateTime? OffboardingDate { get; set; }
    public EmployeeStatus? EmploymentStatus { get; set; }
    public EmploymentType? EmploymentType { get; set; }
    public string? EmployeeLifeCycleStage { get; set; }
    public string? PositionName { get; set; }
    public int? ManagerId { get; set; }
    public ManagerOrCoachSummary? Manager { get; set; }
    public int? DepartmentId { get; set; }
    public int? SubDepartmentId { get; set; }
    public int? CoachId { get; set; }
    public ManagerOrCoachSummary? Coach { get; set; }
    public string? PreviousEmployers { get; set; }
    public string? HierarchyLevel { get; set; }
    public DateTime? HireDate { get; set; }
    public DateTime? ExitDate { get; set; }
    public string? RecruitingSource { get; set; }
}
