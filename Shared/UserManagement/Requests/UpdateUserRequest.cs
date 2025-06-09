namespace Shared.UserManagement.Requests;

public class UpdateUserRequest
{
    public int Id { get; set; }
    public required string GoogleUserId { get; set; }
    public required string Email { get; set; }
    public int RoleId { get; set; } 
    public int EmployeeId { get; set; }
    public string? UserSettings { get; set; }
}