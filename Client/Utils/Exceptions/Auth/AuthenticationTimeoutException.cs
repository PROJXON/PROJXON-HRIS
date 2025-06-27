using System;
using Client.Utils.Enums;

namespace Client.Utils.Exceptions.Auth;

public class AuthenticationTimeoutException(
    string message = "Authentication process timed out",
    string userMessage = "The sign-in process took too long. Please try again.",
    TimeSpan timeout = default,
    Exception? innerException = null)
    : ClientExceptionBase(message, userMessage, ErrorSeverity.Warning, ErrorCategory.Authentication, 
        shouldReport: false, isRetryable: true, 
        suggestedActions: ["Try again", "Check your internet connection"], innerException)
{
    public TimeSpan Timeout { get; } = timeout;
}