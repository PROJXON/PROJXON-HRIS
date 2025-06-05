using CloudSync.Infrastructure;
using CloudSync.Modules.EmployeeManagement.Repositories.Interfaces;
using CloudSync.Modules.EmployeeManagement.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using Shared.EmployeeManagement.Dtos;
using Shared.EmployeeManagement.Models;
using Shared.EmployeeManagement.Requests;

namespace CloudSync.Modules.EmployeeManagement.Repositories;

public class EmployeeRepository(DatabaseContext context) : IEmployeeRepository
{
    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        try
        {
            return await context.Employees.ToListAsync();
        }
        catch (Exception e)
        {
            throw new EmployeeException(e.Message, 500);
        }
    }

    public async Task<IEnumerable<Employee>> GetByDepartmentAsync(string department)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Employee>> GetByRoleAsync(string role)
    {
        throw new NotImplementedException();
    }

    public async Task<Employee> GetByIdAsync(int id)
    {
        var employee = await context.Employees.FindAsync(id);
        
        if (employee == null)
            throw new EmployeeException("Employee with the given ID does not exist", 404);

        return employee;
    }

    public async Task<Employee> CreateAsync(CreateEmployeeRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(int id, EmployeeDto employeeDto)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}