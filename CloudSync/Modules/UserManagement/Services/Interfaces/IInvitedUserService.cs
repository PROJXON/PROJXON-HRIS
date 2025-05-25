using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.UserManagement;
using Shared.Requests.UserManagement;
using Shared.Responses.UserManagement;

namespace CloudSync.Modules.UserManagement.Services.Interfaces;

public interface IInvitedUserService
{
    Task<IEnumerable<InvitedUserResponse>> GetAllAsync();
    Task<ActionResult<InvitedUserResponse>> InviteUserAsync(InvitedUserRequest request);
    Task DeleteInviteAsync(int id);
}