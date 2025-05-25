using Shared.Enums.UserManagement;

namespace Shared.Responses.UserManagement;

public class InvitedUserResponse
{
    public required int Id { get; set; }
    public required string Email { get; set; }
    public required string InvitedByEmployeeId { get; set; }
    public required InvitedUserStatus Status { get; set; }
    public DateTime CreateDateTime { get; set; }
}