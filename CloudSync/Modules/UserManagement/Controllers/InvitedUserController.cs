using Microsoft.AspNetCore.Mvc;
using Shared.Requests.UserManagement;
using Shared.Responses.UserManagement;
using CloudSync.Modules.UserManagement.Services.Interfaces;
namespace CloudSync.Modules.UserManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvitedUserController(IInvitedUserService invitedUserService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvitedUserResponse>>> GetInvitedUsers()
    { 
        var response = await invitedUserService.GetAllAsync();
        return Ok(response);
    }
    
    [HttpPost]
    public async Task<ActionResult<InvitedUserResponse>> InviteUser(InvitedUserRequest request)
    {
        var response = await invitedUserService.InviteUserAsync(request);
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteInvite(int id)
    {
        await invitedUserService.DeleteInviteAsync(id);
        return NoContent();
    }
}