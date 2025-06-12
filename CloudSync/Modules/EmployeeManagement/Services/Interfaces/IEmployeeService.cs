using Shared.EmployeeManagement.Requests;
using Shared.EmployeeManagement.Responses;

namespace CloudSync.Modules.EmployeeManagement.Services.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeResponse>> GetAllAsync();
    Task<IEnumerable<EmployeeResponse>> GetByDepartmentAsync(int id);
    Task<IEnumerable<EmployeeResponse>> GetByRoleAsync(string role);
    Task<EmployeeResponse> GetByIdAsync(int id);
    Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request);
    Task UpdateAsync(int id, UpdateEmployeeRequest request);
    Task DeleteAsync(int id);
}