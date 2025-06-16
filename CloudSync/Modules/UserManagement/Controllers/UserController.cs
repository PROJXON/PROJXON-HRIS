using Microsoft.AspNetCore.Mvc;
using CloudSync.Modules.UserManagement.Services.Interfaces;
using Shared.Responses.UserManagement;
using Shared.UserManagement.Requests;

namespace CloudSync.Modules.UserManagement.Controllers;

[Route("api/[controller]")] [ApiController] public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers()
    {
            var response = await userService.GetAllAsync();
            return Ok(response);
    }
    
    [HttpGet("{id:int}")] public async Task<ActionResult<UserResponse>> GetUser(int id)
    {
            var response = await userService.GetByIdAsync(id);
            return Ok(response);
    }
    
    [HttpPut("{id:int}")] public async Task<ActionResult> PutUser(int id, [FromBody] UpdateUserRequest request)
    {
            await userService.UpdateAsync(id, request);
            return NoContent();
    }
    
    [HttpDelete("{id:int}")] public async Task<ActionResult> DeleteUser(int id)
    {
            await userService.DeleteAsync(id);
            return NoContent();
    }
}

