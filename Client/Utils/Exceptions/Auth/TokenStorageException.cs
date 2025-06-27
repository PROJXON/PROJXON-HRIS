using System;
using Client.Utils.Enums;

namespace Client.Utils.Exceptions.Auth;

public class TokenStorageException(
    string message,
    string userMessage = "Unable to securely store your authentication. Please try again.",
    string? operation = null,
    string? keyName = null,
    Exception? innerException = null)
    : ClientExceptionBase(message, userMessage, ErrorSeverity.Error, ErrorCategory.Storage, 
        shouldReport: true, isRetryable: true, 
        suggestedActions: ["Try again", "Restart the application"], innerException)
{
    public string? Operation { get; } = operation;
    public string? KeyName { get; } = keyName;
}