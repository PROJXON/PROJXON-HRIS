using Shared.Responses.UserManagement;

namespace CloudSync.Modules.UserManagement.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync();
    Task<UserResponse> GetByIdAsync(int id);
    Task<UserResponse> UpdateAsync(int id);
    Task DeleteUserAsync();
}