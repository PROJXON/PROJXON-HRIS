namespace Shared.EmployeeManagement.Responses;

public class EmployeeResponse
{
    public int? Id { get; set; }
    public required EmployeeBasicResponse BasicInfo { get; set; }
    public required EmployeeContactInfoResponse ContactInfo { get; set; }

    public EmployeePositionResponse? PositionDetails { get; set; }
    public EmployeeDocumentsResponse? Documents { get; set; }
    public EmployeeLegalResponse? Legal { get; set; }
    public EmployeeEducationResponse? Education { get; set; }
    public EmployeeTechnicalSpecResponse? TechnicalSpec{ get; set; }
    public EmployeeTrainingResponse? Training { get; set; }
    public DateTime? CreateDateTime { get; set; }
    public DateTime? UpdateDateTime { get; set; }
}