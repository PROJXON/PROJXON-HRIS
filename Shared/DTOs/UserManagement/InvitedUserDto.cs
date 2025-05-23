using Shared.Enums.UserManagement;

namespace Shared.DTOs.UserManagement;

public class InvitedUserDto
{
    public required string Email { get; set; }
    public required string InvitedByEmployeeId { get; set; }
    public InvitedUserStatus Status { get; set; }
}