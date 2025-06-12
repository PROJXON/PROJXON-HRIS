using AutoMapper;
using CloudSync.Modules.EmployeeManagement.Repositories.Interfaces;
using CloudSync.Modules.EmployeeManagement.Services.Interfaces;
using Shared.EmployeeManagement.Models;
using Shared.EmployeeManagement.Requests;
using Shared.EmployeeManagement.Responses;

namespace CloudSync.Modules.EmployeeManagement.Services;

public class EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper) : IEmployeeService
{
    public async Task<IEnumerable<EmployeeResponse>> GetAllAsync()
    {
        var employeeList = await employeeRepository.GetAllAsync();
        List<EmployeeResponse> employeeResponseList = [];

        employeeResponseList.AddRange(employeeList.Select(mapper.Map<EmployeeResponse>));

        return employeeResponseList;
    }

    public async Task<IEnumerable<EmployeeResponse>> GetByDepartmentAsync(int departmentId)
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
        
        var employeeDto = mapper.Map<EmployeeResponse>(employee);
        return mapper.Map<EmployeeResponse>(employeeDto);
    }

    public async Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request)
    {
        var employee = new Employee
        {
            BasicInfo = new EmployeeBasic(),
            ContactInfo = new EmployeeContactInfo(),
            Documents = new EmployeeDocuments(),
            Education = new EmployeeEducation(),
            Legal = new EmployeeLegal(),
            PositionDetails = new EmployeePosition(),
            Training = new EmployeeTraining(),
            CreateDateTime = DateTime.UtcNow,
            UpdateDateTime = DateTime.UtcNow
        };
        
        employee.PositionDetails.Employee = employee;
        employee.Documents.Employee = employee;
        employee.Legal.Employee = employee;
        employee.Education.Employee = employee;
        employee.Training.Employee = employee;
        employee.BasicInfo.FirstName = request.FirstName;
        employee.BasicInfo.LastName = request.LastName;
        
        var createdEmployee = await employeeRepository.CreateAsync(employee);

        return mapper.Map<EmployeeResponse>(createdEmployee);
    }

    public async Task UpdateAsync(int id, UpdateEmployeeRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        

        var updatedEmployee = mapper.Map<Employee>(request);

        await employeeRepository.UpdateAsync(id, updatedEmployee);
    }

    public async Task DeleteAsync(int id)
    {
        await employeeRepository.DeleteAsync(id);
    }
}