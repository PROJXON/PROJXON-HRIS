namespace Shared.DTOs.UserManagement;

public class CreateUserDto
{
    public int Id { get; set; }
    public string GoogleUserId { get; set; }
    public string Email { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime LastLoginDateTime { get; set; }
    public string? UserSettings { get; set; }
}