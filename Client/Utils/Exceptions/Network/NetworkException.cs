using System;
using Client.Utils.Enums;

namespace Client.Utils.Exceptions.Network;

public class NetworkException(
    string message,
    string userMessage = "Unable to connect to the service. Please check your internet connection and try again.",
    int? statusCode = null,
    string? endpoint = null,
    TimeSpan? timeout = null,
    Exception? innerException = null) : ClientExceptionBase(message, userMessage, ErrorSeverity.Error, ErrorCategory.Network, shouldReport: true,
    isRetryable: true, suggestedActions: [ "Check your internet connection.", "Try again later."], innerException)
{
    public int? StatusCode = statusCode;
    public string? Endpoint = endpoint;
    public TimeSpan? Timeout = timeout;
}