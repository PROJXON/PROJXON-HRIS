using CloudSync.Modules.UserManagement.Models;
using Shared.DTOs.UserManagement;

namespace CloudSync.Modules.UserManagement.Repositories;

public interface IInvitedUserRepository
{
    Task<IEnumerable<InvitedUser>> GetAll();
    Task<InvitedUser?> GetByEmail(string email);
    Task<InvitedUserDto> Add(InvitedUserDto invitedUserDto);
    Task<bool> Update(InvitedUserDto invitedUserDto);
    Task<bool> Delete(int id);
}