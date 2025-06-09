using Shared.Responses.UserManagement;
using Shared.UserManagement.Requests;

namespace CloudSync.Modules.UserManagement.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync();
    Task<UserResponse> GetByIdAsync(int id);
    Task UpdateAsync(int id, UpdateUserRequest request);
    Task DeleteAsync(int id);
}