using CloudSync.Exceptions.Business;
using CloudSync.Infrastructure;
using CloudSync.Modules.UserManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.UserManagement.Models;
using Shared.UserManagement.Requests;

namespace CloudSync.Modules.UserManagement.Repositories;

public class UserRepository(DatabaseContext context, IInvitedUserRepository invitedUserRepository) : IUserRepository
{
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await context.Users.ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        var user = await context.Users.FindAsync(id);
        return user;
    }

    public async Task<User?> GetByGoogleUserIdAsync(string googleUserId)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.GoogleUserId == googleUserId);
    }
    
    public async Task<User> CreateUserFromInvitationAsync(InvitedUser invitedUser, string googleUserId)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var newUser = await CreateAsync(invitedUser, googleUserId);
            invitedUserRepository.AcceptInvite(invitedUser);
        
            await transaction.CommitAsync();
            return newUser;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<User> CreateAsync(InvitedUser invitedUser, string googleUserId)
    {
        var newUser = new User
        {
            GoogleUserId = googleUserId, Email = invitedUser.Email, LastLoginDateTime = DateTime.UtcNow, UserSettings = null
        };
        await context.Users.AddAsync(newUser);
        return newUser;
    }

    public async Task UpdateAsync(int id, UpdateUserRequest request)
    {
        var existingUser = await context.Users.FindAsync(id);
        if (existingUser == null)
            throw new EntityNotFoundException("User with the given ID does not exist.");
        existingUser.Email = request.Email;
        existingUser.UserSettings = request.UserSettings;
        
        await context.SaveChangesAsync();
    }

    public async Task UpdateLastLoginTimeAsync(int id)
    {
        var existingUser = await context.Users.FindAsync(id);
        if (existingUser == null)
            throw new EntityNotFoundException("User with the given ID does not exist.");
        
        existingUser.LastLoginDateTime = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null)
        {
            throw new EntityNotFoundException("User with the given ID does not exist.");
        }
        
        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }
}