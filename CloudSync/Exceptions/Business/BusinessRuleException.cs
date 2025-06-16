namespace CloudSync.Exceptions.Business;

// Used for catching scenarios that go against company policy
public class BusinessRuleException : CloudSyncException
{
    public BusinessRuleException(string message) : base(message, 400)
    {
    }

    public BusinessRuleException(string message, Exception innerException) : base(message, innerException, 400)
    {
    }
}