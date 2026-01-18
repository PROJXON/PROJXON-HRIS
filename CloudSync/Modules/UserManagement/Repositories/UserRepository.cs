using CloudSync.Exceptions.Business;
using CloudSync.Infrastructure;
using CloudSync.Modules.UserManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using CloudSync.Modules.UserManagement.Models;
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
    
    /// <summary>
    /// Creates a new User from an invitation with Employee and Role linking.
    /// </summary>
    public async Task<User> CreateUserFromInvitationAsync(
        InvitedUser invitedUser, 
        string googleUserId, 
        int employeeId, 
        int roleId)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var newUser = await CreateAsync(invitedUser, googleUserId, employeeId, roleId);
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
    
    /// <summary>
    /// Legacy overload/Fallback.
    /// </summary>
    public async Task<User> CreateUserFromInvitationAsync(InvitedUser invitedUser, string googleUserId)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // Default to Role 3 (Intern) and 0 EmployeeId if not specified
            var newUser = await CreateAsync(invitedUser, googleUserId, 0, 3);
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

    private async Task<User> CreateAsync(InvitedUser invitedUser, string googleUserId, int employeeId, int roleId)
    {
        var newUser = new User
        {
            GoogleUserId = googleUserId, 
            Email = invitedUser.Email, 
            EmployeeId = employeeId,
            RoleId = roleId,
            LastLoginDateTime = DateTime.UtcNow, 
            UserSettings = null
        };
        await context.Users.AddAsync(newUser);
        await context.SaveChangesAsync();
        return newUser;
    }

    public async Task UpdateAsync(int id, UpdateUserRequest request)
    {
        var existingUser = await context.Users.FindAsync(id);
        if (existingUser == null)
            throw new EntityNotFoundException("User with the given ID does not exist.");
        
        // Update standard fields
        existingUser.Email = request.Email;
        existingUser.UserSettings = request.UserSettings;
        
        // FIXED: Use > 0 check because properties are now non-nullable ints
        if (request.RoleId > 0)
        {
            existingUser.RoleId = request.RoleId;
        }
        
        if (request.EmployeeId > 0)
        {
            existingUser.EmployeeId = request.EmployeeId;
        }
        
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