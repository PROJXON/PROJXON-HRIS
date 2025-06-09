using Shared.Requests.UserManagement;
using Shared.UserManagement.Models;

namespace CloudSync.Modules.UserManagement.Repositories.Interfaces;

public interface IInvitedUserRepository
{
    Task<IEnumerable<InvitedUser>> GetAllAsync();
    Task<InvitedUser?> GetByEmailAsync(string email);
    Task<InvitedUser> AddAsync(InvitedUserRequest invitedUserRequest);
    Task UpdateStatusAsync(int id);
    Task DeleteAsync(int id);
}