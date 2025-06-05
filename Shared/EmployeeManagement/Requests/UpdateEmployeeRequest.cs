using Shared.EmployeeManagement.Models;

namespace Shared.EmployeeManagement.Requests;

public class UpdateEmployeeRequest
{
    public EmployeeBasic BasicInfo { get; set; } = new();
    public EmployeeContactInfo ContactInfo { get; set; } = new();

    public EmployeePosition? PositionDetails { get; set; } = new();
    public EmployeeDocuments? Documents { get; set; } = new();
    public EmployeeLegal? Legal { get; set; } = new();
    public EmployeeEducation? Education { get; set; } = new();
    public EmployeeTraining? Training{ get; set; } = new();
}