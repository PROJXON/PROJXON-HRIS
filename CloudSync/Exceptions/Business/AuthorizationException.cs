namespace CloudSync.Exceptions.Business;

public class AuthorizationException : CloudSyncException
{
    public AuthorizationException(string message) : base(message, 403)
    {
    }

    public AuthorizationException(string message, Exception innerException) : base(message, innerException, 403)
    {
    }
}