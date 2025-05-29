namespace Shared.Requests.UserManagement;

public class InvitedUserRequest
{
    public required string Email { get; set; }
    public required int InvitedByEmployeeId { get; set; }
}