using CloudSync.Modules.EmployeeManagement.Services.Exceptions;
using CloudSync.Modules.EmployeeManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.EmployeeManagement.Requests;
using Shared.EmployeeManagement.Responses;

namespace CloudSync.Modules.EmployeeManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController(IEmployeeService employeeService) : ControllerBase
{
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetAllEmployees()
        {
                        var response = await employeeService.GetAllAsync();
                        return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeResponse>> GetById(int id)
        {
                        var response = await employeeService.GetByIdAsync(id);
                        return Ok(response);
        }

        [HttpGet("department/{departmentId:int}")]
        public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetByDepartment(int departmentId)
        {
                        var response = await employeeService.GetByDepartmentAsync(departmentId);
                        return Ok(response);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutEmployee(int id, [FromBody] UpdateEmployeeRequest request)
        {
                        await employeeService.UpdateAsync(id, request);
                        return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteEmployee(int id)
        {
                        await employeeService.DeleteAsync(id);
                        return NoContent();
        }
}

