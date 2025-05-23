using CloudSync.Infrastructure;
using CloudSync.Modules.UserManagement.Models;
using Shared.DTOs.UserManagement;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CloudSync.Modules.UserManagement.Repositories;

public class InvitedUserRepository(DatabaseContext context) : IInvitedUserRepository
{
    public async Task<IEnumerable<InvitedUser>> GetAll()
    {
        try
        {
            return await context.InvitedUsers.ToListAsync();
        }
        catch (Exception e)
        {
            Log.Error(e, "Exception occurred while fetching users.");
            throw;
        }
    }

    public async Task<InvitedUser?> GetByEmail(string email)
    {
        var emailExists = await context.InvitedUsers.FirstOrDefaultAsync(u => u.Email == email);

        return emailExists;
    }

    public async Task<InvitedUser> Add(InvitedUserDto invitedUserDto)
    {
        try
        {
            var invitedUser = new InvitedUser
            {
                Email = invitedUserDto.Email,
                InvitedByEmployeeId = invitedUserDto.InvitedByEmployeeId,
                Status = invitedUserDto.Status
            };
            
            await context.InvitedUsers.AddAsync(invitedUser);
            await context.SaveChangesAsync();
            
            return invitedUser;
        }
        catch (Exception e)
        {
            Log.Error(e, "Exception occurred while adding user.");
            throw;
        }
    }

    public async Task<bool> Delete(int id)
    {
        try
        {
            var invitedUser = await context.InvitedUsers.FindAsync(id);
            if (invitedUser == null)
            {
                return false;
            }
            
            context.InvitedUsers.Remove(invitedUser);
            await context.SaveChangesAsync();
            
            return true;
        }
        catch (Exception e)
        {
            Log.Error(e, "Exception occurred while deleting user.");
            throw;
        }
    }

    public async Task<bool> Update(InvitedUserDto invitedUserDto)
    {
        try
        {
            var existingUser = await context.InvitedUsers.FindAsync(invitedUserDto.Id);
            if (existingUser == null)
            {
                return false;
            }
            
            var invitedUser = new InvitedUser
            {
                Email = invitedUserDto.Email,
                InvitedByEmployeeId = invitedUserDto.InvitedByEmployeeId,
                Status = invitedUserDto.Status
            };
            
            context.InvitedUsers.Update(invitedUser);
            await context.SaveChangesAsync();
            
            return true;
        }
        catch (Exception e)
        {
            Log.Error(e, "Exception occurred while updating user.");
            throw;
        }
    }
    
}