using System;
using System.Collections.Generic;
using System.Linq;
using Client.Utils.Enums;

namespace Client.Utils.Exceptions;

public abstract class ClientExceptionBase(
    string message,
    string userMessage,
    ErrorSeverity severity = ErrorSeverity.Error,
    ErrorCategory category = ErrorCategory.General,
    bool shouldReport = true,
    bool isRetryable = false,
    IEnumerable<string>? suggestedActions = null,
    Exception? innerException = null)
    : Exception(message, innerException)
{
    public string UserMessage { get; } = userMessage;
    public ErrorSeverity Severity { get; } = severity;
    public ErrorCategory Category { get; } = category;
    public bool ShouldReport { get; } = shouldReport;
    public IReadOnlyList<string> SuggestedActions { get; } = suggestedActions?.ToList().AsReadOnly() ?? new List<string>().AsReadOnly();
    public bool IsRetryable { get; } = isRetryable;
}