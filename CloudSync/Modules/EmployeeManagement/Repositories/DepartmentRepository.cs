using CloudSync.Infrastructure;
using CloudSync.Modules.EmployeeManagement.Repositories.Interfaces;
using CloudSync.Modules.EmployeeManagement.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using CloudSync.Modules.EmployeeManagement.Models;

namespace CloudSync.Modules.EmployeeManagement.Repositories;

public class DepartmentRepository(DatabaseContext context) : IDepartmentRepository
{
    public async Task<IEnumerable<Department>> GetAllAsync()
    {
        try
        {
            return await context.Departments.ToListAsync();
        }
        catch (Exception e)
        {
            throw new EmployeeException(e.Message, 500);
        }
    }
}
