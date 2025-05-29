using CloudSync.Modules.EmployeeManagement.Services.Exceptions;
using CloudSync.Modules.EmployeeManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.EmployeeManagement.Responses;

namespace CloudSync.Modules.EmployeeManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController(IEmployeeService employeeService) : ControllerBase
{
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetAllEmployees()
        {
                try
                {
                        var response = await employeeService.GetAllAsync();
                        return Ok(response);
                }
                catch (EmployeeException e)
                {
                        return StatusCode(e.StatusCode, e.Message);
                }
        }
}

