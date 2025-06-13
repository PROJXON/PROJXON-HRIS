using Shared.EmployeeManagement.Models;

namespace CloudSync.Modules.EmployeeManagement.Repositories.Interfaces;

public interface IDepartmentRepository
{
    Task<IEnumerable<Department>> GetAllAsync();
}
