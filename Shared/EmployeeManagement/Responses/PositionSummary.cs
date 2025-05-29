using Shared.Enums.UserManagement;

namespace Shared.EmployeeManagement.Responses;

public class PositionSummary
{
    public int Id { get; set; }
    public required string PositionName { get; set; }
    public required HierarchyLevel HierarchyLevel { get; set; }
}