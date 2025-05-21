using CloudSync.Modules.UserManagement.Models;
using Shared.DTOs.UserManagement;

namespace CloudSync.Modules.UserManagement.Repositories;

public interface IInvitedUserRespository
{
    Task<IEnumerable<InvitedUser>> GetAll();
    Task<InvitedUser> GetById(int id);
    void Add(InvitedUserDto invitedUserDto);
    void Update(InvitedUser invitedUser);
    void Delete(int id);
}