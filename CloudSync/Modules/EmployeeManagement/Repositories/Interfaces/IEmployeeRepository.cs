using CloudSync.Modules.EmployeeManagement.Models;

namespace CloudSync.Modules.EmployeeManagement.Repositories.Interfaces;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId);
    Task<Employee> GetByIdAsync(int id);
    
    /// <summary>
    /// Finds an employee by personal or company email address.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <returns>The matching employee, or null if not found.</returns>
    Task<Employee?> GetByEmailAsync(string email);
    
    Task<Employee> CreateAsync(Employee employee);
    Task UpdateAsync(int id, Employee employee);
    Task DeleteAsync(int id);
}
