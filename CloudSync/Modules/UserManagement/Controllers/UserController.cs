using Microsoft.AspNetCore.Mvc;
using CloudSync.Modules.UserManagement.Services.Exceptions;
using CloudSync.Modules.UserManagement.Services.Interfaces;
using Shared.DTOs.UserManagement;
using Shared.Responses.UserManagement;

namespace CloudSync.Modules.UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
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
        
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetUser(int id)
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
        
        [HttpPut("{id}")]
        public async Task<ActionResult> PutUser(int id, [FromBody] UserDto userDto)
        {
            try
            {
                await userService.UpdateAsync(id, userDto);
                return NoContent();
            }
            catch (UserException e)
            {
                return StatusCode(e.StatusCode, new { message = e.Message });
            }
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
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
}
