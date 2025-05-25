using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.UserManagement;
using Shared.Requests.UserManagement;
using Shared.Responses.UserManagement;

namespace CloudSync.Modules.UserManagement.Services.Interfaces;

public interface IInvitedUserService
{
    Task<IEnumerable<InviteUserResponse>> GetAllAsync();
    Task<ActionResult<InviteUserResponse>> InviteUserAsync(InviteUserRequest request);
}