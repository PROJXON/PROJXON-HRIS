namespace CloudSync.Exceptions.Infrastructure;

public class ExternalServiceException : CloudSyncException
{
    public ExternalServiceException(string message) : base(message, 502)
    {
    }

    public ExternalServiceException(string message, Exception innerException) : base(message, innerException, 502)
    {
    }
}