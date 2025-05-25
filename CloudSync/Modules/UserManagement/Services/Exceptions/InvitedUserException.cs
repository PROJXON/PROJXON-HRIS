namespace CloudSync.Modules.UserManagement.Services.Exceptions;

public class InvitedUserException(string message, int statusCode = 400) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}