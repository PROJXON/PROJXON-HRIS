using Shared.EmployeeManagement.Responses;

namespace Shared.EmployeeManagement.BaseDtos;

public class EmployeePositionBase
{
    public int Id { get; set; }
    public string? PositionName { get; set; }
    public int? DepartmentId { get; set; }
    public int? SubDepartmentId { get; set; }
    public string? HierarchyLevel { get; set; }
    public ManagerOrCoachSummary? Manager { get; set; }
    public ManagerOrCoachSummary? Coach { get; set; }
    public DateTime? OnboardingDate { get; set; }
    public DateTime? OffboardingDate { get; set; }
    public string? EmploymentStatus { get; set; }
    public string? EmploymentType { get; set; }
    public string? RecruitingSource { get; set; }
}
