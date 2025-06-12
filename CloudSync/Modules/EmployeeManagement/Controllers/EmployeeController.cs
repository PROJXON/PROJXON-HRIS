using CloudSync.Modules.EmployeeManagement.Services.Exceptions;
using CloudSync.Modules.EmployeeManagement.Services.Interfaces;
using CloudSync.Modules.UserManagement.Services.Exceptions;
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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeResponse>> GetById(int id)
        {
                try
                {
                        var response = await employeeService.GetByIdAsync(id);
                        return Ok(response);
                }
                catch (EmployeeException e)
                {
                        return StatusCode(e.StatusCode, e.Message);
                }
        }

        [HttpGet("department/{departmentId:int}")]
        public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetByDepartment(int departmentId)
        {
                try
                {
                        var response = await employeeService.GetByDepartmentAsync(departmentId);
                        return Ok(response);
                }
                catch (EmployeeException e)
                {
                        return StatusCode(e.StatusCode, new { message = e.Message });
                }
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeResponse>> CreateEmployee([FromBody] CreateEmployeeRequest request)
        {
                try
                {
                        var response = await employeeService.CreateAsync(request);
                        return Ok(response);
                }
                catch (EmployeeException e)
                {
                        return StatusCode(e.StatusCode, new { message = e.Message });
                }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutEmployee(int id, [FromBody] UpdateEmployeeRequest request)
        {
                try
                {
                        await employeeService.UpdateAsync(id, request);
                        return NoContent();
                }
                catch (UserException e)
                {
                        return StatusCode(e.StatusCode, new { message = e.Message });
                }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteEmployee(int id)
        {
                try
                {
                        await employeeService.DeleteAsync(id);
                        return NoContent();
                }
                catch (UserException e)
                {
                        return StatusCode(e.StatusCode, new { message = e.Message });
                }
        }
}

