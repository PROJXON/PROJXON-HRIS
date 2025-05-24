using Google.Apis.Auth;
using Shared.Requests.UserManagement;
using Shared.Responses.UserManagement;

namespace CloudSync.Modules.UserManagement.Services.Interfaces;

public interface IAuthService
{
    Task<GoogleLoginResponse> LoginAsync(GoogleLoginRequest request);
    Task<GoogleJsonWebSignature.Payload> ValidateIdToken(GoogleLoginRequest request);
    string GenerateJwt(string email);
}