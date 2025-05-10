namespace Shared.DTOs;

public class UserDTO
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime LastLoginDateTime { get; set; }
    public required string UserSettings { get; set; }
}