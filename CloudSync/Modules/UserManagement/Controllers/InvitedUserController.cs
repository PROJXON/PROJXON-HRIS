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
    [HttpPost("invite-user")]
    public async Task<ActionResult<InviteUserResponse>> InviteUser(InviteUserRequest request)
    {
        try
        {
            var response = await invitedUserService.InviteUser(request);
            return Ok(response);
        }
        catch (InvitedUserException e)
        {
            return StatusCode(e.StatusCode, new { message = e.Message });
        }
    }
}