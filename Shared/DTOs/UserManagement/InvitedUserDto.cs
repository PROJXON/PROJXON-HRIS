using Shared.Enums.UserManagement;

namespace Shared.DTOs.UserManagement;

public class InvitedUserDto
{
    public int? Id { get; set; }
    public required string Email { get; set; }
    public required string InvitedByEmployeeId { get; set; }
    public InvitedUserStatus Status { get; set; }
    public DateTime? CreateDateTime { get; set; }
}