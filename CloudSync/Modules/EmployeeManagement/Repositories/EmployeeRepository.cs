using CloudSync.Infrastructure;
using CloudSync.Modules.EmployeeManagement.Models;
using CloudSync.Modules.EmployeeManagement.Repositories.Interfaces;
using CloudSync.Modules.EmployeeManagement.Services.Exceptions;
using CloudSync.Modules.UserManagement.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using Shared.EmployeeManagement.Dtos;

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
        throw new NotImplementedException();
    }

    public async Task<Employee> CreateAsync(EmployeeDto employeeDto)
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