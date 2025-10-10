using AutoMapper;
using CloudSync.Exceptions.Business;
using CloudSync.Modules.UserManagement.Repositories.Interfaces;
using CloudSync.Modules.UserManagement.Services.Interfaces;
using Google.Apis.Auth;
using Shared.Enums.UserManagement;
using Shared.Requests.UserManagement;
using Shared.Responses.UserManagement;
using CloudSync.Modules.UserManagement.Models;

namespace CloudSync.Modules.UserManagement.Services;

public class AuthService(IInvitedUserRepository invitedUserRepository, IUserRepository userRepository, IGoogleTokenValidator googleTokenValidator, IJwtTokenService jwtTokenService, IMapper mapper) : IAuthService
{
    public async Task<GoogleLoginResponse> LoginAsync(GoogleLoginRequest request)
    {
        ValidateLoginRequest(request);
    
        var payload = await googleTokenValidator.ValidateAsync(request);
    
        var existingUser = await userRepository.GetByGoogleUserIdAsync(payload.Subject);
        if (existingUser != null)
        {
            return await LoginExistingUserAsync(existingUser);
        }
        
        //Temporarily authenticate user without invitation
        var dummyInvitee = new InvitedUser
        {
            InvitedByEmployeeId = 1,
            Email = payload.Email,
            Status = "Pending",
            CreateDateTime = DateTime.UtcNow
        };
    
        var newUser = await userRepository.CreateUserFromInvitationAsync(dummyInvitee, payload.Subject);
    
        return new GoogleLoginResponse
        {
            JsonWebToken = jwtTokenService.GenerateToken(payload.Email),
            ExpiresIn = 3600,
            User = mapper.Map<UserResponse>(newUser)
        };
        
        //Uncomment when ready to implement
        // return await HandleInvitedUserLoginAsync(payload);
    }
    
    private static void ValidateLoginRequest(GoogleLoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.IdToken))
            throw new ValidationException("Missing ID token.");
    }
    
    private async Task<GoogleLoginResponse> LoginExistingUserAsync(User existingUser)
    {
        var existingUserResponse = mapper.Map<UserResponse>(existingUser);

        await userRepository.UpdateLastLoginTimeAsync(existingUser.Id);

        return new GoogleLoginResponse
        {
            JsonWebToken = jwtTokenService.GenerateToken(existingUser.Email),
            ExpiresIn = 3600,
            User = existingUserResponse
        };
    }
    
    private async Task<GoogleLoginResponse> HandleInvitedUserLoginAsync(GoogleJsonWebSignature.Payload payload)
    {
        var existingInvitee = await GetValidatedInviteeAsync(payload.Email);
        var invitationStatus = ParseInvitationStatus(existingInvitee.Status);
        
        return await ProcessInvitationStatusAsync(existingInvitee, payload, invitationStatus);
    }
    
    private async Task<InvitedUser> GetValidatedInviteeAsync(string email)
    {
        var existingInvitee = await invitedUserRepository.GetByEmailAsync(email);
        
        if (existingInvitee == null)
        {
            throw new EntityNotFoundException("User has not been invited.");
        }
        
        return existingInvitee;
    }
    
    private static InvitedUserStatus ParseInvitationStatus(string statusString)
    {
        if (!Enum.TryParse(statusString, out InvitedUserStatus status))
        {
            throw new AuthenticationException("Invalid status value");
        }
        
        return status;
    }
    
    private async Task<GoogleLoginResponse> ProcessInvitationStatusAsync(InvitedUser invitee, GoogleJsonWebSignature.Payload payload, InvitedUserStatus status)
    {
        return status switch
        {
            InvitedUserStatus.Accepted => throw new DuplicateEntityException("User has already accepted invitation."),
            InvitedUserStatus.Pending => await CreateUserFromInvitationAsync(invitee, payload),
            _ => throw new AuthenticationException("Error logging in or creating new user.")
        };
    }
    
    private async Task<GoogleLoginResponse> CreateUserFromInvitationAsync(InvitedUser invitee, GoogleJsonWebSignature.Payload payload)
    {
        var newUser = await userRepository.CreateUserFromInvitationAsync(invitee, payload.Subject);
    
        return new GoogleLoginResponse
        {
            JsonWebToken = jwtTokenService.GenerateToken(payload.Email),
            ExpiresIn = 3600,
            User = mapper.Map<UserResponse>(newUser)
        };
    }
}