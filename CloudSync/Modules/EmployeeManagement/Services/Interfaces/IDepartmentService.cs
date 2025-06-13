using Shared.EmployeeManagement.Responses;

namespace CloudSync.Modules.EmployeeManagement.Services.Interfaces;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentResponse>> GetAllAsync();
}
