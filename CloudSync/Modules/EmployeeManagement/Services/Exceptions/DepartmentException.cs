namespace CloudSync.Modules.EmployeeManagement.Services.Exceptions;

public class DepartmentException(string message, int statusCode = 400) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}