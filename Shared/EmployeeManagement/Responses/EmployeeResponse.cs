using Shared.EmployeeManagement.Models;

namespace Shared.EmployeeManagement.Responses;

public class EmployeeResponse
{
    public int? Id { get; set; }
    public EmployeeBasic BasicInfo { get; set; } = new();
    public EmployeeContactInfo ContactInfo { get; set; } = new();

    public EmployeePosition? PositionDetails { get; set; } = new();
    public EmployeeDocuments? Documents { get; set; } = new();
    public EmployeeLegal? Legal { get; set; } = new();
    public EmployeeEducation? Education { get; set; } = new();
    public EmployeeTraining? Training{ get; set; } = new();
    public DateTime? CreateDateTime { get; set; }
    public DateTime? UpdateDateTime { get; set; }
}