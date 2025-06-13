using CloudSync.Modules.EmployeeManagement.Repositories.Interfaces;
using Shared.EmployeeManagement.Models;

namespace CloudSync.Modules.EmployeeManagement.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    public async Task<IEnumerable<Department>> GetAllAsync()
    {
        throw new NotImplementedException();
    }
}
