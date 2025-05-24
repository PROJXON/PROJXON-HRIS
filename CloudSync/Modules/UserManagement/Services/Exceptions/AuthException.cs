namespace CloudSync.Modules.UserManagement.Services;

public class AuthException(string message, int statusCode = 400) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}