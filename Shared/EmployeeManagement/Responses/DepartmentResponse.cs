using Shared.EmployeeManagement.BaseDtos;
using Shared.EmployeeManagement.Models;

namespace Shared.EmployeeManagement.Responses;

public class DepartmentResponse : DepartmentBase
{
    public Department? ParentDepartment { get; set; }
}
