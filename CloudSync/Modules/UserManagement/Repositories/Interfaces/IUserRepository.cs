using CloudSync.Modules.UserManagement.Models;
using Shared.DTOs.UserManagement;

namespace CloudSync.Modules.UserManagement.Repositories.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetByIdAsync(int id);
    void CreateAsync(UserDto userDto);
    void UpdateAsync(UserDto userDto);
    void DeleteAsync(string googleUserId);
}