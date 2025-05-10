namespace Shared.DTOs.User;

public class CreateUserDTO
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime LastLoginDateTime { get; set; }
    public string? UserSettings { get; set; }
}