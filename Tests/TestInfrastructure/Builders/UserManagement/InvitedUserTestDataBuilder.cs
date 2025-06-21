using CloudSync.Modules.UserManagement.Models;
using Shared.Enums.UserManagement;

namespace Tests.TestInfrastructure.Builders.UserManagement;

public class InvitedUserTestDataBuilder
{
    private  int _id = 1;
    private string _email = "test@example.com";
    private int _invitedByEmployeeId = 20;
    private InvitedUserStatus _status = InvitedUserStatus.Pending;

    public InvitedUserTestDataBuilder WithId(int id)
    {
        _id = id;
        return this;
    }
    
    public InvitedUserTestDataBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }
    
    public InvitedUserTestDataBuilder WithInvitedByEmployeeId(int invitedByEmployeeId)
    {
        _invitedByEmployeeId = invitedByEmployeeId;
        return this;
    }

    public InvitedUserTestDataBuilder WithStatus(InvitedUserStatus status)
    {
        _status = status;
        return this;
    }

    public InvitedUser Build()
    {
        return new InvitedUser
        {
            Id = _id,
            Email = _email,
            InvitedByEmployeeId = _invitedByEmployeeId,
            Status = _status.ToString()
        };
    }
}