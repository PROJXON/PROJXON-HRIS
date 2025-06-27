using System;
using Client.Utils.Enums;

namespace Client.Utils.Exceptions.Auth;

public class AuthenticationException(
    string message,
    string userMessage = "Please sign in again to continue",
    AuthenticationTokenType tokenType = AuthenticationTokenType.AccessToken,
    bool requiresRelogin = true,
    Exception? innerException = null)
    : ClientExceptionBase(message,
        userMessage,
        ErrorSeverity.Warning,
        ErrorCategory.Authentication,
        shouldReport: true,
        isRetryable: false,
        suggestedActions: requiresRelogin ? ["Sign in again"] : ["Try again"],
        innerException: innerException)
{
    public AuthenticationTokenType TokenType { get; } = tokenType;
    public bool RequiresRelogin { get; } = requiresRelogin;
}