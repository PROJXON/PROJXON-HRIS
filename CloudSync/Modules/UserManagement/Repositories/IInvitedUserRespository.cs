using CloudSync.Modules.UserManagement.Models;

namespace CloudSync.Modules.UserManagement.Repositories;

public interface IInvitedUserRespository
{
    IEnumerable<InvitedUser> GetAll();
    InvitedUser GetById(int id);
    void Add(InvitedUser invitedUser);
    void Update(InvitedUser invitedUser);
    void Delete(int id);
}