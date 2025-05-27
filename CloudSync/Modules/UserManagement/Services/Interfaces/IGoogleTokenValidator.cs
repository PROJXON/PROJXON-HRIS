using Google.Apis.Auth;
using Shared.Requests.UserManagement;

namespace CloudSync.Modules.UserManagement.Services.Interfaces;

public interface IGoogleTokenValidator
{
    Task<GoogleJsonWebSignature.Payload> ValidateAsync(GoogleLoginRequest request);
}