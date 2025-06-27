using System;
using Client.Utils.Enums;

namespace Client.Utils.Exceptions.Auth;

public class TokenExpiredException(
    string message = "Authentication token has expired",
    string userMessage = "Your session has expired. Please sign in again.",
    AuthenticationTokenType tokenType = AuthenticationTokenType.AccessToken,
    Exception? innerException = null)
    : AuthenticationException(message, userMessage, tokenType, requiresRelogin: true, innerException)
{
    public DateTime ExpiredAt { get; init; } = DateTime.UtcNow;
}