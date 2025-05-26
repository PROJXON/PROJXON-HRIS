namespace Shared.Responses.UserManagement;

public class GoogleLoginResponse
{
    public required string JsonWebToken { get; set; }
    public UserResponse? User { get; set; }
}