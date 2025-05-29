using AutoMapper;
using CloudSync.Modules.EmployeeManagement.Repositories.Interfaces;
using CloudSync.Modules.EmployeeManagement.Services.Interfaces;
using Shared.EmployeeManagement.Requests;
using Shared.EmployeeManagement.Responses;

namespace CloudSync.Modules.EmployeeManagement.Services;

public class EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper) : IEmployeeService
{
    public async Task<IEnumerable<EmployeeResponse>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<EmployeeResponse>> GetByDepartmentAsync(string department)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<EmployeeResponse>> GetByRoleAsync(string role)
    {
        throw new NotImplementedException();
    }

    public async Task<EmployeeResponse> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(int id, UpdateEmployeeRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}