using Microsoft.AspNetCore.Mvc;
using Shared.Requests.UserManagement;
using Shared.Responses.UserManagement;
using CloudSync.Modules.UserManagement.Services.Interfaces;
using CloudSync.Modules.UserManagement.Services.Exceptions;

namespace CloudSync.Modules.UserManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvitedUserController(IInvitedUserService invitedUserService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvitedUserResponse>>> GetInvitedUsers()
    {
        try
        {
            var response = await invitedUserService.GetAllAsync();
            return Ok(response);
        }
        catch (InvitedUserException e)
        {
            return StatusCode(e.StatusCode, new { message = e.Message });
        }
    }
    
    [HttpPost("invite-user")]
    public async Task<ActionResult<InvitedUserResponse>> InviteUser(InvitedUserRequest request)
    {
        try
        {
            var response = await invitedUserService.InviteUserAsync(request);
            return Ok(response);
        }
        catch (InvitedUserException e)
        {
            return StatusCode(e.StatusCode, new { message = e.Message });
        }
    }

    [HttpDelete("delete-invite/{id}")]
    public async Task<ActionResult> DeleteInvite(int id)
    {
        try
        {
            await invitedUserService.DeleteInviteAsync(id);
            return NoContent();
        }
        catch (InvitedUserException e)
        {
            return StatusCode(e.StatusCode, new { message = e.Message });
        }
    }
}