using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using CloudSync.Exceptions.Business;
using CloudSync.Exceptions.Infrastructure;
using CloudSync.Modules.UserManagement.Repositories.Interfaces;
using CloudSync.Modules.UserManagement.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Shared.Enums.UserManagement;
using Shared.Requests.UserManagement;
using Shared.Responses.UserManagement;
using Shared.UserManagement.Models;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace CloudSync.Modules.UserManagement.Services;

public class AuthService(IInvitedUserRepository invitedUserRepository, IUserRepository userRepository, IGoogleTokenValidator googleTokenValidator, IJwtTokenService jwtTokenService, IMapper mapper) : IAuthService
{
    public async Task<GoogleLoginResponse> LoginAsync(GoogleLoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.IdToken))
            throw new ValidationException("Missing ID token.");
        
        var payload = await googleTokenValidator.ValidateAsync(request);
        
        
        var existingUser = await userRepository.GetByGoogleUserIdAsync(payload.Subject);

        if (existingUser != null)
        {
            return await LoginExistingUserAsync(existingUser);
        }
        
        var existingInvitee = await invitedUserRepository.GetByEmailAsync(payload.Email);
        
        if (existingInvitee == null)
        {
            throw new EntityNotFoundException("User has not been invited.");
        }

        return Enum.Parse<InvitedUserStatus>(existingInvitee.Status) switch
        {
            InvitedUserStatus.Accepted => throw new DuplicateEntityException("User has already accepted invitation."), // should never happen, existing user should be logged in above
            InvitedUserStatus.Pending => new GoogleLoginResponse
            {
                JsonWebToken = jwtTokenService.GenerateToken(payload.Email),
                User = await CreateUserFromInviteAsync(existingInvitee, payload.Subject)
            },
            _ => throw new AuthenticationException("Error logging in or creating new user.")
        };
    }

    private async Task<GoogleLoginResponse> LoginExistingUserAsync(User existingUser)
    {
        var existingUserResponse = mapper.Map<UserResponse>(existingUser);

        await userRepository.UpdateLastLoginTimeAsync(existingUser.Id);

        return new GoogleLoginResponse
        {
            JsonWebToken = jwtTokenService.GenerateToken(existingUser.Email),
            User = existingUserResponse
        };
    }
    
    private async Task<UserResponse> CreateUserFromInviteAsync(InvitedUser invitedUser, string googleUserId)
    {
        var newUser = await userRepository.CreateAsync(invitedUser, googleUserId);
        await invitedUserRepository.UpdateStatusAsync(invitedUser.Id);

        return mapper.Map<UserResponse>(newUser);
    }

    
}