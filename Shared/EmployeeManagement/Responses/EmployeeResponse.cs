using Shared.EmployeeManagement.Models;

namespace Shared.EmployeeManagement.Responses;

public class EmployeeResponse
{
    public int? Id { get; set; }
    public required EmployeeBasic BasicInfo { get; set; }
    public required EmployeeContactInfo ContactInfo { get; set; }

    public EmployeePositionResponse? PositionDetails { get; set; }
    public EmployeeDocumentsResponse? Documents { get; set; }
    public EmployeeLegalResponse? Legal { get; set; }
    public EmployeeEducationResponse? Education { get; set; }
    public EmployeeTrainingResponse? Training{ get; set; }
    public DateTime? CreateDateTime { get; set; }
    public DateTime? UpdateDateTime { get; set; }
}