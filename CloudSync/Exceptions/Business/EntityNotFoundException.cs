namespace CloudSync.Exceptions.Business;

public class EntityNotFoundException : CloudSyncException
{
    public EntityNotFoundException(string message) : base(message, 404)
    {
    }

    public EntityNotFoundException(string message, Exception innerException) : base(message, innerException, 404)
    {
    }
}