namespace Shared.DTOs.UserManagement;

public class UserDto
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime LastLoginDateTime { get; set; }
    public string? UserSettings { get; set; }
}