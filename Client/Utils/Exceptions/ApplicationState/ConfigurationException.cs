using System;
using Client.Utils.Enums;

namespace Client.Utils.Exceptions.ApplicationState;

public class ConfigurationException(
    string message,
    string userMessage,
    string configurationMissing,
    Exception? innerException = null) : ClientExceptionBase(message, userMessage, ErrorSeverity.Critical,
    ErrorCategory.Configuration, shouldReport: true,
    isRetryable: false, suggestedActions: ["Please contact support."], innerException)
{
    public string ConfigurationMissing = configurationMissing;
}