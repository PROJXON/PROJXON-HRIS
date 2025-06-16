namespace CloudSync.Exceptions;

public class CloudSyncException : Exception
{
    public int StatusCode { get; }

    public CloudSyncException(string message, int statusCode = 500) : base(message)
    {
        StatusCode = statusCode;
    }

    public CloudSyncException(string message, Exception innerException, int statusCode = 500) : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}