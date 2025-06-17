namespace CloudSync.Modules.UserManagement.Services.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(string email);
}