using AutoMapper;
using CloudSync.Exceptions.Business;
using CloudSync.Modules.UserManagement.Repositories.Interfaces;
using CloudSync.Modules.UserManagement.Services.Interfaces;
using CloudSync.Modules.EmployeeManagement.Repositories.Interfaces;
using Google.Apis.Auth;
using Shared.Enums.UserManagement;
using Shared.Requests.UserManagement;
using Shared.Responses.UserManagement;
using CloudSync.Modules.UserManagement.Models;
using CloudSync.Modules.EmployeeManagement.Models;

namespace CloudSync.Modules.UserManagement.Services;

public class AuthService(
    IInvitedUserRepository invitedUserRepository, 
    IUserRepository userRepository, 
    IEmployeeRepository employeeRepository,
    IGoogleTokenValidator googleTokenValidator, 
    IJwtTokenService jwtTokenService, 
    IMapper mapper) : IAuthService
{
    // Default RoleId for new users (Intern)
    private const int DefaultRoleId = 3;
    
    public async Task<GoogleLoginResponse> LoginAsync(GoogleLoginRequest request)
    {
        ValidateLoginRequest(request);
    
        var payload = await googleTokenValidator.ValidateAsync(request);
    
        // Check if user already exists
        var existingUser = await userRepository.GetByGoogleUserIdAsync(payload.Subject);
        if (existingUser != null)
        {
            return await LoginExistingUserAsync(existingUser);
        }
        
        // For new users, strictly require invitation
        return await HandleInvitedUserLoginAsync(payload);
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
        // Strictly check for invitation - throw if not found
        var existingInvitee = await GetValidatedInviteeAsync(payload.Email);
        var invitationStatus = ParseInvitationStatus(existingInvitee.Status);
        
        return await ProcessInvitationStatusAsync(existingInvitee, payload, invitationStatus);
    }
    
    private async Task<InvitedUser> GetValidatedInviteeAsync(string email)
    {
        var existingInvitee = await invitedUserRepository.GetByEmailAsync(email);
        
        if (existingInvitee == null)
        {
            throw new EntityNotFoundException("User has not been invited. Please contact HR to request access.");
        }
        
        return existingInvitee;
    }
    
    private static InvitedUserStatus ParseInvitationStatus(string statusString)
    {
        if (!Enum.TryParse(statusString, out InvitedUserStatus status))
        {
            throw new AuthenticationException("Invalid invitation status value.");
        }
        
        return status;
    }
    
    private async Task<GoogleLoginResponse> ProcessInvitationStatusAsync(
        InvitedUser invitee, 
        GoogleJsonWebSignature.Payload payload, 
        InvitedUserStatus status)
    {
        return status switch
        {
            InvitedUserStatus.Accepted => throw new DuplicateEntityException("User has already accepted invitation."),
            InvitedUserStatus.Pending => await CreateUserFromInvitationAsync(invitee, payload),
            _ => throw new AuthenticationException("Error logging in or creating new user.")
        };
    }
    
    private async Task<GoogleLoginResponse> CreateUserFromInvitationAsync(
        InvitedUser invitee, 
        GoogleJsonWebSignature.Payload payload)
    {
        // Check if an Employee record exists for this email
        var existingEmployee = await employeeRepository.GetByEmailAsync(payload.Email);
        
        int employeeId;
        if (existingEmployee != null)
        {
            // Use existing employee
            employeeId = existingEmployee.Id;
        }
        else
        {
            // Create new Employee with default data from Google payload
            var newEmployee = CreateEmployeeFromGooglePayload(payload);
            var createdEmployee = await employeeRepository.CreateAsync(newEmployee);
            employeeId = createdEmployee.Id;
        }
        
        // Create the User record linked to the Employee with RoleId = 3 (Intern)
        var newUser = await userRepository.CreateUserFromInvitationAsync(
            invitee, 
            payload.Subject, 
            employeeId, 
            DefaultRoleId);
    
        return new GoogleLoginResponse
        {
            JsonWebToken = jwtTokenService.GenerateToken(payload.Email),
            ExpiresIn = 3600,
            User = mapper.Map<UserResponse>(newUser)
        };
    }
    
    /// <summary>
    /// Creates a new Employee entity populated with data from the Google OAuth payload.
    /// </summary>
    private static Employee CreateEmployeeFromGooglePayload(GoogleJsonWebSignature.Payload payload)
    {
        var now = DateTime.UtcNow;
        
        return new Employee
        {
            BasicInfo = new EmployeeBasic
            {
                FirstName = payload.GivenName ?? "New",
                LastName = payload.FamilyName ?? "Employee",
            },
            ContactInfo = new EmployeeContactInfo
            {
                PersonalEmail = payload.Email,
            },
            CreateDateTime = now,
            UpdateDateTime = now
        };
    }
}
