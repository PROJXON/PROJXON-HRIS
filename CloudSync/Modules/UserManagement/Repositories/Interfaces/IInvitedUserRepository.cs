using CloudSync.Modules.UserManagement.Models;
using Shared.DTOs.UserManagement;

namespace CloudSync.Modules.UserManagement.Repositories.Interfaces;

public interface IInvitedUserRepository
{
    Task<IEnumerable<InvitedUser>> GetAllAsync();
    Task<InvitedUser?> GetByEmail(string email);
    Task<InvitedUser> Add(InvitedUserDto invitedUserDto);
    Task DeleteInviteAsync(string id);
}