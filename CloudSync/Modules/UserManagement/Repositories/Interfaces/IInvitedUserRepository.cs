using Shared.Requests.UserManagement;
using CloudSync.Modules.UserManagement.Models;

namespace CloudSync.Modules.UserManagement.Repositories.Interfaces;

public interface IInvitedUserRepository
{
    Task<IEnumerable<InvitedUser>> GetAllAsync();
    Task<InvitedUser?> GetByEmailAsync(string email);
    Task<InvitedUser> AddAsync(InvitedUserRequest invitedUserRequest);
    void AcceptInvite(InvitedUser invitedUser);
    Task<bool> DeleteAsync(int id);
}