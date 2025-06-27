using System;
using System.Collections.Generic;
using System.Linq;
using Client.Utils.Enums;

namespace Client.Utils.Exceptions;

public abstract class ClientExceptionBase : Exception
{
    public string UserMessage { get; }
    public ErrorSeverity Severity { get; }
    public ErrorCategory Category { get; }
    public bool ShouldReport { get; }
    public IReadOnlyList<string> SuggestedActions { get; }
    public IReadOnlyDictionary<string, object>? Context { get; }
    public bool IsRetryable { get; }

    protected ClientExceptionBase(
        string message,
        string userMessage,
        ErrorSeverity severity = ErrorSeverity.Error,
        ErrorCategory category = ErrorCategory.General,
        bool shouldReport = true,
        bool isRetryable = false,
        IEnumerable<string>? suggestedActions = null,
        Dictionary<string, object>? context = null,
        Exception? innerException = null
    ) : base(message, innerException)
    {
        UserMessage = userMessage;
        Severity = severity;
        Category = category;
        ShouldReport = shouldReport;
        IsRetryable = isRetryable;
        SuggestedActions = suggestedActions?.ToList().AsReadOnly() ?? new List<string>().AsReadOnly();
        Context = context?.AsReadOnly();
    }
    
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