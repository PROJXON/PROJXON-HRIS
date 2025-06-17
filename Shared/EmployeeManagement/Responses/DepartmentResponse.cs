using Shared.EmployeeManagement.BaseDtos;

namespace Shared.EmployeeManagement.Responses;

public class DepartmentResponse : DepartmentBase
{
    public DepartmentResponse? ParentDepartment { get; set; }
}
