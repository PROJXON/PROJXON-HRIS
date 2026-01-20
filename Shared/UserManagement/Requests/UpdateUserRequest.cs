namespace Shared.UserManagement.Requests;

public class UpdateUserRequest
{
    /// <summary>
    /// The ID of the user to update.
    /// </summary>
    public int Id { get; set; }

    public required string GoogleUserId { get; set; }
    
    public required string Email { get; set; }
    
    public string? UserSettings { get; set; }
    
    /// <summary>
    /// Optional RoleId to update the user's role.
    /// </summary>
    public int RoleId { get; set; }
    
    /// <summary>
    /// Optional EmployeeId to link/update the user's employee record.
    /// </summary>
    public int EmployeeId { get; set; }
}