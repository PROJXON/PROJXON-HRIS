using System;
using System.Collections.Generic;
using Client.Utils.Enums;

namespace Client.Utils.Exceptions.Auth;

public class AuthenticationException : ClientExceptionBase
{
    public AuthenticationTokenType TokenType { get; }
    public bool RequiresRelogin { get; }

    public AuthenticationException(
        string message,
        string userMessage = "Please sign in again to continue",
        AuthenticationTokenType tokenType = AuthenticationTokenType.AccessToken,
        bool requiresRelogin = true,
        Exception? innerException = null
    ) : base(
        message,
        userMessage,
        ErrorSeverity.Warning,
        ErrorCategory.Authentication,
        shouldReport: true,
        isRetryable: false,
        suggestedActions: requiresRelogin ? new[] { "Sign in again" } : new[] { "Try again" },
        innerException: innerException)
    {
        TokenType = tokenType;
        RequiresRelogin = requiresRelogin;
    }

    protected override ClientExceptionBase CreateCopy(Dictionary<string, object>? context = null, string? correlationId = null)
    {
        return new AuthenticationException(
            Message,
            UserMessage,
            TokenType,
            RequiresRelogin,
            InnerException);
    }
}