namespace CloudSync.Exceptions.Business;

public class ValidationException : CloudSyncException
{
    public ValidationException(string message) : base(message, 400)
    {
    }

    public ValidationException(string message, Exception innerException) : base(message, innerException, 400)
    {
    }
}