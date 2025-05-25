using Microsoft.AspNetCore.Mvc;
using Shared.Requests.UserManagement;
using Shared.Responses.UserManagement;

namespace CloudSync.Modules.UserManagement.Services.Interfaces;

public interface IInvitedUserService
{
    Task<ActionResult<InviteUserResponse>> InviteUser(InviteUserRequest request);
}