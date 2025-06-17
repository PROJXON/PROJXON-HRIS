namespace CloudSync.Exceptions.Business;

public class AuthenticationException : CloudSyncException
{
    public AuthenticationException(string message) : base(message, 401)
    {
    }

    public AuthenticationException(string message, Exception innerException) : base(message, innerException, 401)
    {
    }
}