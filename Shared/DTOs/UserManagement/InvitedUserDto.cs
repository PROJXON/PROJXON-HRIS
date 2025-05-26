using Shared.Enums.UserManagement;

namespace Shared.DTOs.UserManagement;

public class InvitedUserDto
{
    public required string Email { get; set; }
    public required int InvitedByEmployeeId { get; set; }
    public required string Status { get; set; }
}