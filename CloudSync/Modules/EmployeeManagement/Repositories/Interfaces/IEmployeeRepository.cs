using Shared.EmployeeManagement.Dtos;
using Shared.EmployeeManagement.Models;
using Shared.EmployeeManagement.Requests;

namespace CloudSync.Modules.EmployeeManagement.Repositories.Interfaces;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<IEnumerable<Employee>> GetByDepartmentAsync(string department);
    Task<IEnumerable<Employee>> GetByRoleAsync(string role);
    Task<Employee> GetByIdAsync(int id);
    Task<Employee> CreateAsync(CreateEmployeeRequest request);
    Task UpdateAsync(int id, EmployeeDto employeeDto);
    Task DeleteAsync(int id);
}