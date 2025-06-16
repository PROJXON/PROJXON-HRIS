namespace CloudSync.Exceptions.Infrastructure;

public class DataAccessException : CloudSyncException
{
    public DataAccessException(string message, int statusCode = 400) : base(message, statusCode)
    {
    }

    public DataAccessException(string message, Exception innerException, int statusCode = 400) : base(message, innerException, statusCode)
    {
    }
}