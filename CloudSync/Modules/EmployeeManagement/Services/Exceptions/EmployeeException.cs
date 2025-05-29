namespace CloudSync.Modules.EmployeeManagement.Services.Exceptions;

public class EmployeeException(string message, int statusCode = 400) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}