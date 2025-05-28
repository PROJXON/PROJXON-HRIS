using CloudSync.Modules.UserManagement.Models;
using Shared.DTOs.UserManagement;
using Shared.Requests.UserManagement;

namespace CloudSync.Modules.UserManagement.Repositories.Interfaces;

public interface IInvitedUserRepository
{
    Task<IEnumerable<InvitedUser>> GetAllAsync();
    Task<InvitedUser?> GetByEmailAsync(string email);
    Task<InvitedUser> AddAsync(InvitedUserRequest invitedUserRequest);
    Task UpdateStatusAsync(int id);
    Task DeleteAsync(int id);
}