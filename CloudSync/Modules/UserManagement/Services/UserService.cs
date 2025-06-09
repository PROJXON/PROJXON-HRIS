using AutoMapper;
using CloudSync.Modules.UserManagement.Repositories.Interfaces;
using CloudSync.Modules.UserManagement.Services.Interfaces;
using Shared.Responses.UserManagement;
using Shared.UserManagement.Requests;

namespace CloudSync.Modules.UserManagement.Services;

public class UserService(IUserRepository userRepository, IMapper mapper) : IUserService
{
    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        var userList = await userRepository.GetAllAsync();
        List<UserResponse> userResponseList = [];
        
        userResponseList.AddRange(userList.Select(mapper.Map<UserResponse>));

        return userResponseList;
    }

    public async Task<UserResponse> GetByIdAsync(int id)
    {
        var user = await userRepository.GetByIdAsync(id);
        return mapper.Map<UserResponse>(user);
    }

    public async Task UpdateAsync(int id, UpdateUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        await userRepository.UpdateAsync(id, request);
    }

    public async Task DeleteAsync(int id)
    {
        await userRepository.DeleteAsync(id);
    }
}