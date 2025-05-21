namespace Shared.Requests.UserManagement;

public class CreateUserRequest
{
    public int Id { get; set; }
    public required string GoogleUserId { get; set; }
    public required string Email { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime LastLoginDateTime { get; set; }
    public string? UserSettings { get; set; }
}