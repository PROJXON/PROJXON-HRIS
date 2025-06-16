namespace CloudSync.Exceptions.Business;

public class ValidationException : CloudSyncException
{
    public ValidationException(string message, int statusCode = 400) : base(message, statusCode)
    {
    }

    public ValidationException(string message, Exception innerException, int statusCode = 400) : base(message, innerException, statusCode)
    {
    }
}