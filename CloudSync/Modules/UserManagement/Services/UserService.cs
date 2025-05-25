using CloudSync.Modules.UserManagement.Repositories.Interfaces;
using CloudSync.Modules.UserManagement.Services.Interfaces;
using Shared.DTOs.UserManagement;
using Shared.Responses.UserManagement;

namespace CloudSync.Modules.UserManagement.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        var userList = await userRepository.GetAllAsync();
        List<UserResponse> userResponseList = [];
            
        userResponseList.AddRange(userList.Select(user => new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            CreateDateTime = user.CreateDateTime,
            LastLoginDateTime = user.LastLoginDateTime,
            UserSettings = user.UserSettings
        }));

        return userResponseList;
    }

    public async Task<UserResponse> GetByIdAsync(int id)
    {
        var user = await userRepository.GetByIdAsync(id);
        
        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            CreateDateTime = user.CreateDateTime,
            LastLoginDateTime = user.LastLoginDateTime,
            UserSettings = user.UserSettings
        };
    }

    public async Task<UserResponse> UpdateAsync(int id, UserDto user)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteUserAsync()
    {
        throw new NotImplementedException();
    }
}