using System;
using System.Threading.Tasks;
using Client.Utils.Classes;

namespace Client.Services;

public interface IAuthenticationService
{
    event EventHandler<AuthenticationChangedEventArgs>? AuthenticationChanged;
    bool IsAuthenticated { get; }
    string? CurrentUserEmail { get; } // Added for Dev Mode logic
    Task<bool> LoginAsync();
    Task LogoutAsync();
}