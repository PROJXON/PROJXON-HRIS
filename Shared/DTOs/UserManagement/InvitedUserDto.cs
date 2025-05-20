namespace Shared.DTOs.UserManagement;

public class InvitedUserDto
{
    public required string Email { get; set; }
    public required int InvitedById { get; set; }
    public required string Status { get; set; }
}