using AutoMapper;
using CloudSync.Exceptions.Business;
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
        return userList.Select(mapper.Map<UserResponse>);
    }

    public async Task<UserResponse> GetByIdAsync(int id)
    {
        var user = await userRepository.GetByIdAsync(id);
        
        if (user == null)
            throw new EntityNotFoundException("User with the given ID does not exist.");
        
        return mapper.Map<UserResponse>(user);
    }

    public async Task UpdateAsync(int id, UpdateUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (id != request.Id)
        {
            throw new ValidationException("The provided ID does not match the user ID.");
        }
        
        await userRepository.UpdateAsync(id, request);
    }

    public async Task DeleteAsync(int id)
    {
        await userRepository.DeleteAsync(id);
    }
}