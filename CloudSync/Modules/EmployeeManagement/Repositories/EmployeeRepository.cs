using CloudSync.Infrastructure;
using CloudSync.Modules.EmployeeManagement.Repositories.Interfaces;
using CloudSync.Modules.EmployeeManagement.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using Shared.EmployeeManagement.Models;

namespace CloudSync.Modules.EmployeeManagement.Repositories;

public class EmployeeRepository(DatabaseContext context) : IEmployeeRepository
{
    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        try
        {
            return await context.Employees
                .Include(e => e.Documents)
                .Include(e => e.Education)
                .Include(e => e.Legal)
                .Include(e => e.PositionDetails)
                .Include(e => e.Training)
                .ToListAsync();
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
        try
        {
            var employee = await context.Employees
                .Include(e => e.Documents)
                .Include(e => e.Education)
                .Include(e => e.Legal)
                .Include(e => e.PositionDetails)
                .Include(e => e.Training)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
                throw new EmployeeException("Employee with the given ID does not exist", 404);

            return employee;
        }
        catch (Exception e)
        {
            throw new EmployeeException(e.Message, 500);
        }
    }

    public async Task<Employee> CreateAsync(Employee employee)
    {
        try
        {
            await context.Employees.AddAsync(employee);
            await context.SaveChangesAsync();

            return employee;
        }
        catch (Exception e)
        {
            throw new EmployeeException(e.Message, 500);
        }
    }

    public async Task UpdateAsync(int id, Employee employee)
    {
        try
        {
            if (id != employee.Id)
            {
                throw new EmployeeException("The provided ID does not match the employee ID.");
            }

            var existingEmployee = await context.Employees
                .Include(e => e.Documents)
                .Include(e => e.Education)
                .Include(e => e.Legal)
                .Include(e => e.PositionDetails)
                .Include(e => e.Training)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (existingEmployee == null)
                throw new EmployeeException("Employee with the given ID does not exist.", 404);

            existingEmployee.UpdateDateTime = DateTime.UtcNow;
            existingEmployee.BasicInfo = employee.BasicInfo;
            existingEmployee.ContactInfo = employee.ContactInfo;
            existingEmployee.Documents = employee.Documents;
            existingEmployee.Education = employee.Education;
            // existingEmployee.Legal = employee.Legal;
            // existingEmployee.PositionDetails = employee.PositionDetails;
            // existingEmployee.Training = employee.Training;

            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new EmployeeException(e.Message, 500);
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var employee = await context.Employees
                .Include(e => e.Documents)
                .Include(e => e.Education)
                .Include(e => e.Legal)
                .Include(e => e.PositionDetails)
                .Include(e => e.Training)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null)
            {
                throw new EmployeeException("Employee with the given ID does not exist", 404);
            }

            context.Employees.Remove(employee);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new EmployeeException(e.Message, 500);
        }
    }
}
