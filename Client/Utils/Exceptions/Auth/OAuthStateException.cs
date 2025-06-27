using System;
using Client.Utils.Enums;

namespace Client.Utils.Exceptions.Auth;

public class OAuthStateException(
    string message = "Invalid OAuth state parameter",
    string userMessage = "Authentication request appears to be tampered with. Please try again.",
    string? expectedState = null,
    string? receivedState = null,
    Exception? innerException = null)
    : ClientExceptionBase(message, userMessage, ErrorSeverity.Error, ErrorCategory.Security, 
        shouldReport: true, isRetryable: false, 
        suggestedActions: ["Try signing in again", "Clear application cache"], innerException)
{
    public string? ExpectedState { get; } = expectedState;
    public string? ReceivedState { get; } = receivedState;
}