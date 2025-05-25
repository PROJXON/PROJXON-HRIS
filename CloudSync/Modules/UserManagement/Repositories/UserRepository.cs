using CloudSync.Infrastructure;
using CloudSync.Modules.UserManagement.Models;
using CloudSync.Modules.UserManagement.Repositories.Interfaces;
using CloudSync.Modules.UserManagement.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.UserManagement;

namespace CloudSync.Modules.UserManagement.Repositories;

public class UserRepository(DatabaseContext context) : IUserRepository
{
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        try
        {
            return await context.Users.ToListAsync();
        }
        catch (Exception e)
        {
            throw new UserException(e.Message, 500);
        }
    }

    public Task<User> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public void CreateAsync(UserDto userDto)
    {
        throw new NotImplementedException();
    }

    public void UpdateAsync(UserDto userDto)
    {
        throw new NotImplementedException();
    }

    public void DeleteAsync(string googleUserId)
    {
        throw new NotImplementedException();
    }
}