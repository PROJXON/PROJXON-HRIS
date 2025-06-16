namespace CloudSync.Exceptions.Business;

public class DuplicateEntityException : CloudSyncException
{
    public DuplicateEntityException(string message) : base(message, 409)
    {
    }

    public DuplicateEntityException(string message, Exception innerException) : base(message, innerException, 409)
    {
    }
}