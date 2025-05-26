using Shared.Requests.UserManagement;
using Shared.Responses.UserManagement;

namespace CloudSync.Modules.UserManagement.Services.Interfaces;

public interface IAuthService
{
    Task<GoogleLoginResponse> LoginAsync(GoogleLoginRequest request);
}