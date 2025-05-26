using CloudSync.Modules.UserManagement.Models;
using Shared.DTOs.UserManagement;

namespace CloudSync.Modules.UserManagement.Repositories.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetByIdAsync(int id);
    Task<User?> GetByGoogleUserIdAsync(string googleUserId);
    Task CreateAsync(UserDto userDto);
    Task UpdateAsync(int id, UserDto userDto);
    Task DeleteAsync(int id);
}