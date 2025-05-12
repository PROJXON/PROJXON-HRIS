using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CloudSync.Modules.UserManagement.Services;

public class AuthService(IConfiguration configuration)
{
    public async Task<bool> ValidatePassword(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password);
    }

    public string GenerateJWT(string username)
    {
        var jwtSettings = configuration.GetSection("JWT");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: [new Claim(ClaimTypes.Name, username)],
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiresInMinutes"])),
            signingCredentials: creds
        );
            
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}