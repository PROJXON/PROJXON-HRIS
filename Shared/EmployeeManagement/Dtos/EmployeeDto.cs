using Shared.EmployeeManagement.Models;

namespace Shared.EmployeeManagement.Dtos;

public class EmployeeDto
{
    public int? Id { get; set; }
    public EmployeeBasic? BasicInfo { get; set; }
    public EmployeeContactInfo? ContactInfo { get; set; }

    public EmployeePosition? PositionDetails { get; set; }
    public EmployeeDocuments? Documents { get; set; }
    public EmployeeLegal? Legal { get; set; }
    public EmployeeEducation? Education { get; set; }
    public EmployeeTraining? Training{ get; set; }
    public DateTime? CreateDateTime { get; set; }
    public DateTime? UpdateDateTime { get; set; }
}