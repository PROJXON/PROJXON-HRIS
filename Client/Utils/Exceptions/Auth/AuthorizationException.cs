using System;
using Client.Utils.Enums;

namespace Client.Utils.Exceptions.Auth;

public class AuthorizationException(
    string message,
    string userMessage = "Authorization failed. Please try signing in again.",
    string? error = null,
    string? errorDescription = null,
    string? errorUri = null,
    Exception? innerException = null)
    : ClientExceptionBase(message, userMessage, ErrorSeverity.Warning, ErrorCategory.Authentication, 
        shouldReport: true, isRetryable: false, 
        suggestedActions: ["Try signing in again", "Check your internet connection"], innerException)
{
    public string? Error { get; } = error;
    public string? ErrorDescription { get; } = errorDescription;
    public string? ErrorUri { get; } = errorUri;
}