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
            throw new InvitedUserException(e.Message, 500);
        }
    }

    public async Task DeleteInviteAsync(string id)
    {
        try
        {
            var invitedUser = await context.InvitedUsers.FindAsync(id);
            if (invitedUser == null)
                throw new InvitedUserException("Invite does not exist.", 400);
            
            context.InvitedUsers.Remove(invitedUser);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new InvitedUserException(e.Message, 500);
        }
    }
}