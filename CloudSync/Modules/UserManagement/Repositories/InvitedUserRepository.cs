using CloudSync.Infrastructure;
using CloudSync.Modules.UserManagement.Models;
using CloudSync.Modules.UserManagement.Repositories.Interfaces;
using CloudSync.Modules.UserManagement.Services.Exceptions;
using Shared.DTOs.UserManagement;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CloudSync.Modules.UserManagement.Repositories;

public class InvitedUserRepository(DatabaseContext context) : IInvitedUserRepository
{
    public async Task<IEnumerable<InvitedUser>> GetAllAsync()
    {
        try
        {
            return await context.InvitedUsers.ToListAsync();
        }
        catch (Exception e)
        {
            throw new InvitedUserException(e.Message, 500);
        }
    }
    
    public async Task<InvitedUser?> GetByEmailAsync(string email)
    {
        try
        {
            return await context.InvitedUsers.FirstOrDefaultAsync(u => u.Email == email);
        }
        catch (Exception e)
        {
            throw new InvitedUserException(e.Message, 500);
        }
    }

    public async Task<InvitedUser> AddAsync(InvitedUserDto invitedUserDto)
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

    public async Task DeleteAsync(int id)
    {
        try
        {
            var existingInvite = await context.InvitedUsers.FindAsync(id);
            if (existingInvite == null)
                throw new InvitedUserException("No invite exists for this user.", 404);
            
            context.InvitedUsers.Remove(existingInvite);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new InvitedUserException(e.Message, 500);
        }
    }
}