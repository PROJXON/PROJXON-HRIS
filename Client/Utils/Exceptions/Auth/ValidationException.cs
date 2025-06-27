using System;
using System.Collections.Generic;
using System.Linq;
using Client.Utils.Enums;

namespace Client.Utils.Exceptions.Auth;

public class ValidationException(
    string message,
    string userMessage,
    IReadOnlyDictionary<string, string[]> validationErrors,
    string? fieldName,
    Exception? innerException = null)
    : ClientExceptionBase(message, userMessage, ErrorSeverity.Warning, ErrorCategory.Validation, shouldReport: false, isRetryable: false, suggestedActions:
            ["Please check your input and try again"],
        innerException)
{
    public IReadOnlyDictionary<string, string[]> ValidationErrors { get; } = validationErrors;
    public string? FieldName { get; } = fieldName;
}