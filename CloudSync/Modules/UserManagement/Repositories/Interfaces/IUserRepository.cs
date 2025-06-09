using Shared.UserManagement.Models;
using Shared.UserManagement.Requests;

namespace CloudSync.Modules.UserManagement.Repositories.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetByIdAsync(int id);
    Task<User?> GetByGoogleUserIdAsync(string googleUserId);
    Task<User> CreateAsync(InvitedUser invitedUser, string googleUserId);
    Task UpdateAsync(int id, UpdateUserRequest request);
    Task UpdateLastLoginTimeAsync(int id);
    Task DeleteAsync(int id);
}