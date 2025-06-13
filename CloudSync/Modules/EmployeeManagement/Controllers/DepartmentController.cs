using CloudSync.Modules.EmployeeManagement.Services.Interfaces;
using Shared.EmployeeManagement.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CloudSync.Modules.EmployeeManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DepartmentController(IDepartmentService departmentService) : ControllerBase
{
    [HttpGet]
    public Task<ActionResult<IEnumerable<DepartmentResponse>>> GetAllDepartments()
    {

    }
}
