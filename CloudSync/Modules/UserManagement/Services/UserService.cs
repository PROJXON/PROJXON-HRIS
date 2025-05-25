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

    public async Task UpdateAsync(int id, UserDto userDto)
    {
        ArgumentNullException.ThrowIfNull(userDto);
        
        await userRepository.UpdateAsync(id, userDto);
    }

    public async Task DeleteUserAsync()
    {
        throw new NotImplementedException();
    }
}