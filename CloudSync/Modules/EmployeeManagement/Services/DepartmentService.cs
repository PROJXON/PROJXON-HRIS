using CloudSync.Modules.EmployeeManagement.Services.Interfaces;
using Shared.EmployeeManagement.Responses;

namespace CloudSync.Modules.EmployeeManagement.Services;

public class DepartmentService : IDepartmentService
{
    public async Task<IEnumerable<DepartmentResponse>> GetAllAsync()
    {
        throw new NotImplementedException();
    }
}
