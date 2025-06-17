using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CloudSync.Exceptions.Infrastructure;
using CloudSync.Modules.UserManagement.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace CloudSync.Modules.UserManagement.Services;

public class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    private readonly IConfigurationSection _jwtSettings = configuration.GetSection("JWT");
    
    public string GenerateToken(string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings["Key"] ?? throw new ConfigurationException("Jwt key not found or missing.")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _jwtSettings["Issuer"],
            audience: _jwtSettings["Audience"],
            claims: [new Claim(ClaimTypes.Name, email)],
            expires: DateTime.UtcNow.AddMinutes(double.Parse(_jwtSettings["ExpiresInMinutes"] ?? throw new ConfigurationException("Expiration time not found or missing."))),
            signingCredentials: creds
        );
            
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}