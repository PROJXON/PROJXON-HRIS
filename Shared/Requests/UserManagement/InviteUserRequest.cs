namespace Shared.Requests.UserManagement;

public class InviteUserRequest
{
    public required string Email { get; set; }
    public required string InvitedByEmployeeId { get; set; }
}