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

    public async Task<User?> GetByGoogleUserIdAsync(string googleUserId)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.GoogleUserId == googleUserId);
    }

    public async Task<User> CreateAsync(InvitedUser invitedUser, string googleUserId)
    {
        try
        {
            var newUser = new User
            {
                GoogleUserId = googleUserId,
                Email = invitedUser.Email,
                LastLoginDateTime = DateTime.UtcNow,
                UserSettings = null
            };

            await context.Users.AddAsync(newUser);
            await context.SaveChangesAsync();

            return newUser;
        }
        catch (Exception e)
        {
            throw new UserException(e.Message, 500);
        }
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

    public async Task UpdateLastLoginTimeAsync(int id)
    {
        var existingUser = await context.Users.FindAsync(id);
        if (existingUser == null)
            throw new UserException("User with the given ID does not exist.", 404);
        
        existingUser.LastLoginDateTime = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                throw new UserException("User with the given ID does not exist.", 404);
            }
            
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new UserException(e.Message, 500);
        }
    }
}