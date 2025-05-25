using CloudSync.Modules.UserManagement.Models;
using Shared.DTOs.UserManagement;

namespace CloudSync.Modules.UserManagement.Repositories.Interfaces;

public interface IInvitedUserRepository
{
    Task<IEnumerable<InvitedUser>> GetAllAsync();
    Task<InvitedUser?> GetByEmailAsync(string email);
    Task<InvitedUser> AddAsync(InvitedUserDto invitedUserDto);
    Task DeleteAsync(int id);
}