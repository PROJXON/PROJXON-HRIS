namespace Shared.Responses.UserManagement;

public class GoogleLoginResponse
{
    public required string JsonWebToken { get; set; }
    public int ExpiresIn { get; set; } = 3600;
    public UserResponse? User { get; set; }
}