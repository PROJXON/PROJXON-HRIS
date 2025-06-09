using Microsoft.AspNetCore.Mvc;
using CloudSync.Modules.UserManagement.Services.Exceptions;
using CloudSync.Modules.UserManagement.Services.Interfaces;
using Shared.Responses.UserManagement;
using Shared.UserManagement.Requests;

namespace CloudSync.Modules.UserManagement.Controllers;

[Route("api/[controller]")] [ApiController] public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers()
    {
        try
        {
            var response = await userService.GetAllAsync();
            return Ok(response);
        }
        catch (UserException e)
        {
            return StatusCode(e.StatusCode, new { message = e.Message });
        }
    }
    
    [HttpGet("{id:int}")] public async Task<ActionResult<UserResponse>> GetUser(int id)
    {
        try
        {
            var response = await userService.GetByIdAsync(id);
            return Ok(response);
        }
        catch (UserException e)
        {
            return StatusCode(e.StatusCode, new { message = e.Message });
        }
    }
    
    [HttpPut("{id:int}")] public async Task<ActionResult> PutUser(int id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            await userService.UpdateAsync(id, request);
            return NoContent();
        }
        catch (UserException e)
        {
            return StatusCode(e.StatusCode, new { message = e.Message });
        }
    }
    
    [HttpDelete("{id:int}")] public async Task<ActionResult> DeleteUser(int id)
    {
        try
        {
            await userService.DeleteAsync(id);
            return NoContent();
        }
        catch (UserException e)
        {
            return StatusCode(e.StatusCode, new { message = e.Message });
        }
    }
}

