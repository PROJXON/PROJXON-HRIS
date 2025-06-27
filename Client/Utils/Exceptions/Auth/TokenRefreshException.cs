using System;
using Client.Utils.Enums;

namespace Client.Utils.Exceptions.Auth;

public class TokenRefreshException(
    string message,
    string userMessage = "Unable to refresh your session. Please sign in again.",
    int? statusCode = null,
    string? errorDescription = null,
    Exception? innerException = null)
    : AuthenticationException(message, userMessage, AuthenticationTokenType.RefreshToken, requiresRelogin: true, innerException)
{
    public int? StatusCode { get; } = statusCode;
    public string? ErrorDescription { get; } = errorDescription;
}