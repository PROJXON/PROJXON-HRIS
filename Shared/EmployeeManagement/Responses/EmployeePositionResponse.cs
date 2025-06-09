using Shared.EmployeeManagement.Models;

namespace Shared.EmployeeManagement.Responses;

public class EmployeePositionResponse
{
    public int Id { get; set; }
    public string? PositionName { get; set; }
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public int? SubDepartmentId { get; set; }
    public Department? SubDepartment { get; set; }
    public string? HierarchyLevel { get; set; }
    public ManagerOrCoachSummary? Manager { get; set; }
    public ManagerOrCoachSummary? Coach { get; set; }
    public DateTime? OnboardingDate { get; set; }
    public DateTime? OffboardingDate { get; set; }
    public string? EmploymentStatus { get; set; }
    public string? EmploymentType { get; set; }
    public string? RecruitingSource { get; set; }
}