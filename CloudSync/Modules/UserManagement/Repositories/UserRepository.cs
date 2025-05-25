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

    public async Task<User> GetByIdAsync(int id)
    {
        try
        {
            var user = await context.Users.FindAsync(id);

            if (user == null)
                throw new UserException("User with the given ID does not exist.", 404);

            return user;
        }
        catch (Exception e)
        {
            throw new UserException(e.Message, 500);
        }
    }

    public async Task CreateAsync(UserDto userDto)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(int id, UserDto userDto)
    {
        try
        {
            if (id != userDto.Id)
            {
                throw new UserException("The provided ID does not match the user ID.");
            }

            var existingUser = await context.Users.FindAsync(id);
            if (existingUser == null)
                throw new UserException("User with the given ID does not exist.", 404);

            existingUser.Email = userDto.Email;
            existingUser.UserSettings = userDto.UserSettings;
                
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new UserException(e.Message, 500);
        }
    }

    public async Task DeleteAsync(string googleUserId)
    {
        throw new NotImplementedException();
    }
}