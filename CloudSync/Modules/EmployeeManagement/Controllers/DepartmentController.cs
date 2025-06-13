using CloudSync.Modules.EmployeeManagement.Services.Exceptions;
using CloudSync.Modules.EmployeeManagement.Services.Interfaces;
using Shared.EmployeeManagement.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CloudSync.Modules.EmployeeManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DepartmentController(IDepartmentService departmentService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentResponse>>> GetAllDepartments()
    {
        try
        {
            var response = await departmentService.GetAllAsync();

            return Ok(response);
        }
        catch (DepartmentException e)
        {
            return StatusCode(e.StatusCode, e.Message);
        }
    }
}
