using CloudSync.Exceptions.Business;
using CloudSync.Infrastructure;
using CloudSync.Modules.UserManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Enums.UserManagement;
using Shared.Requests.UserManagement;
using Shared.UserManagement.Models;

namespace CloudSync.Modules.UserManagement.Repositories;

public class InvitedUserRepository(DatabaseContext context) : IInvitedUserRepository
{
    public async Task<IEnumerable<InvitedUser>> GetAllAsync()
    {
            return await context.InvitedUsers.ToListAsync();
    }
    
    public async Task<InvitedUser?> GetByEmailAsync(string email)
    {
            return await context.InvitedUsers.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<InvitedUser> AddAsync(InvitedUserRequest invitedUserRequest)
    {
            var invitedUser = new InvitedUser
            {
                Email = invitedUserRequest.Email,
                InvitedByEmployeeId = invitedUserRequest.InvitedByEmployeeId,
                Status = nameof(InvitedUserStatus.Pending)
            };
            
            await context.InvitedUsers.AddAsync(invitedUser);
            await context.SaveChangesAsync();
            
            return invitedUser;
    }
    public async Task UpdateStatusAsync(int id)
    {
            var existingInvite = await context.InvitedUsers.FindAsync(id);
            if (existingInvite == null)
                throw new EntityNotFoundException("No invite exists for this user.");

            existingInvite.Status = nameof(InvitedUserStatus.Pending);
            
            context.InvitedUsers.Update(existingInvite);
            await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
            var existingInvite = await context.InvitedUsers.FindAsync(id);
            if (existingInvite == null)
                throw new EntityNotFoundException("No invite exists for this user.");
            
            context.InvitedUsers.Remove(existingInvite);
            await context.SaveChangesAsync();
    }
}