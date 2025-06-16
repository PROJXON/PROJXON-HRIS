namespace CloudSync.Exceptions.Infrastructure;

public class DataAccessException : CloudSyncException
{
    public DataAccessException(string message) : base(message)
    {
    }

    public DataAccessException(string message, Exception innerException) : base(message, innerException)
    {
    }
}