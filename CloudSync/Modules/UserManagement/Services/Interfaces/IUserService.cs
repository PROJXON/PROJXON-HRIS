using Shared.Responses.UserManagement;

namespace CloudSync.Modules.UserManagement.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync();
    Task<UserResponse> GetByIdAsync();
    Task DeleteUserAsync();
}