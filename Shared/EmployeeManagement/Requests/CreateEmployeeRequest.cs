namespace Shared.EmployeeManagement.Requests;

public class CreateEmployeeRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}