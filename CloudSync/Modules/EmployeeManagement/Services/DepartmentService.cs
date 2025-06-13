using AutoMapper;
using CloudSync.Modules.EmployeeManagement.Repositories.Interfaces;
using CloudSync.Modules.EmployeeManagement.Services.Interfaces;
using Shared.EmployeeManagement.Responses;

namespace CloudSync.Modules.EmployeeManagement.Services;

public class DepartmentService(IDepartmentRepository departmentRepository, IMapper mapper) : IDepartmentService
{
    public async Task<IEnumerable<DepartmentResponse>> GetAllAsync()
    {
        var departments = await departmentRepository.GetAllAsync();
        List<DepartmentResponse> departmentResponseList = [];
        
        departmentResponseList.AddRange(departments.Select(mapper.Map<DepartmentResponse>));

        return departmentResponseList;
    }
}
