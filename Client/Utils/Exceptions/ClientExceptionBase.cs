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
    Dictionary<string, object>? context = null,
    Exception? innerException = null)
    : Exception(message, innerException)
{
    public string UserMessage { get; } = userMessage;
    public ErrorSeverity Severity { get; } = severity;
    public ErrorCategory Category { get; } = category;
    public bool ShouldReport { get; } = shouldReport;
    public IReadOnlyList<string> SuggestedActions { get; } = suggestedActions?.ToList().AsReadOnly() ?? new List<string>().AsReadOnly();
    public IReadOnlyDictionary<string, object>? Context { get; } = context?.AsReadOnly();
    public bool IsRetryable { get; } = isRetryable;

    public virtual ClientExceptionBase WithContext(string key, object value)
    {
        var newContext = Context?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, object>();
        newContext[key] = value;
        return CreateCopy(newContext);
    }
    
    protected abstract ClientExceptionBase CreateCopy(
        Dictionary<string, object>? context = null, 
        string? correlationId = null);
}