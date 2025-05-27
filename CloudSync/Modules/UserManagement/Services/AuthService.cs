using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CloudSync.Modules.UserManagement.Models;
using CloudSync.Modules.UserManagement.Repositories.Interfaces;
using CloudSync.Modules.UserManagement.Services.Exceptions;
using CloudSync.Modules.UserManagement.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Google.Apis.Auth;
using Shared.Enums.UserManagement;
using Shared.Requests.UserManagement;
using Shared.Responses.UserManagement;

namespace CloudSync.Modules.UserManagement.Services;

public class AuthService(IConfiguration configuration, IInvitedUserRepository invitedUserRepository, IUserRepository userRepository, IGoogleTokenValidator googleTokenValidator) : IAuthService
{
    private readonly IConfigurationSection _jwtSettings = configuration.GetSection("JWT");

    public async Task<GoogleLoginResponse> LoginAsync(GoogleLoginRequest request)
    {
        var payload = await googleTokenValidator.ValidateAsync(request);
        
        if (string.IsNullOrWhiteSpace(request.IdToken))
        {
            throw new AuthException("Missing ID token.");
        }
        
        var existingUser = await userRepository.GetByGoogleUserIdAsync(payload.Subject);

        if (existingUser != null)
        {
            return await LoginExistingUserAsync(existingUser);
        }
        
        var existingInvitee = await invitedUserRepository.GetByEmailAsync(payload.Email);
        
        if (existingInvitee == null)
        {
            throw new AuthException("User has not been invited.", 404);
        }

        return Enum.Parse<InvitedUserStatus>(existingInvitee.Status) switch
        {
            InvitedUserStatus.Accepted => throw new AuthException("User has already accepted invitation."), // should never happen, existing user should be logged in above
            InvitedUserStatus.Expired => throw new AuthException("Invitation has expired."),
            InvitedUserStatus.Pending => new GoogleLoginResponse
            {
                JsonWebToken = GenerateJwt(payload.Email),
                User = await CreateUserFromInviteAsync(existingInvitee, payload.Subject)
            },
            _ => throw new AuthException("Error logging in or creating new user.", 500)
        };
    }

    private async Task<GoogleLoginResponse> LoginExistingUserAsync(User existingUser)
    {
        var existingUserResponse = new UserResponse
        {
            Id = existingUser.Id,
            Email = existingUser.Email,
            CreateDateTime = existingUser.CreateDateTime,
            LastLoginDateTime = DateTime.UtcNow,
            UserSettings = existingUser.UserSettings
        };

        await userRepository.UpdateLastLoginTimeAsync(existingUser.Id);

        return new GoogleLoginResponse
        {
            JsonWebToken = GenerateJwt(existingUser.Email),
            User = existingUserResponse
        };
    }
    
    private async Task<UserResponse> CreateUserFromInviteAsync(InvitedUser invitedUser, string googleUserId)
    {
        var newUser = await userRepository.CreateAsync(invitedUser, googleUserId);

        return new UserResponse
        {
            Id = newUser.Id,
            Email = newUser.Email,
            CreateDateTime = newUser.CreateDateTime,
            LastLoginDateTime = newUser.LastLoginDateTime,
            UserSettings = newUser.UserSettings
        };
    }

    

    private string GenerateJwt(string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings["Key"] ?? throw new AuthException("Jwt key not found or missing.", 500)));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _jwtSettings["Issuer"],
            audience: _jwtSettings["Audience"],
            claims: [new Claim(ClaimTypes.Name, email)],
            expires: DateTime.UtcNow.AddMinutes(double.Parse(_jwtSettings["ExpiresInMinutes"] ?? throw new AuthException("Jwt key not found or missing.", 500))),
            signingCredentials: creds
        );
            
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}