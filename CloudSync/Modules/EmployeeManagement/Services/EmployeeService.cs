using AutoMapper;
using CloudSync.Modules.EmployeeManagement.Repositories.Interfaces;
using CloudSync.Modules.EmployeeManagement.Services.Exceptions;
using CloudSync.Modules.EmployeeManagement.Services.Interfaces;
using Shared.EmployeeManagement.Dtos;
using Shared.EmployeeManagement.Requests;
using Shared.EmployeeManagement.Responses;

namespace CloudSync.Modules.EmployeeManagement.Services;

public class EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper) : IEmployeeService
{
    public async Task<IEnumerable<EmployeeResponse>> GetAllAsync()
    {
        var employeeList = await employeeRepository.GetAllAsync();
        List<EmployeeDto> employeeDtoList = [];
        List<EmployeeResponse> employeeResponseList = [];

        employeeDtoList.AddRange(employeeList.Select(mapper.Map<EmployeeDto>));
        employeeResponseList.AddRange(employeeDtoList.Select(mapper.Map<EmployeeResponse>));

        return employeeResponseList;
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
        var employee = await employeeRepository.GetByIdAsync(id);
        
        var employeeDto = mapper.Map<EmployeeDto>(employee);
        return mapper.Map<EmployeeResponse>(employeeDto);
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