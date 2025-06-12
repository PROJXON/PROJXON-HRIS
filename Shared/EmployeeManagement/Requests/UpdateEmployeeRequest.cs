using Shared.EmployeeManagement.Models;

namespace Shared.EmployeeManagement.Requests;

public class UpdateEmployeeRequest
{
    public int Id { get; set; }
    public EmployeeBasic? BasicInfo { get; set; }
    public EmployeeContactInfo? ContactInfo { get; set; }

    public EmployeePositionRequest? PositionDetails { get; set; }
    public EmployeeDocumentsRequest? Documents { get; set; }
    public EmployeeLegalRequest? Legal { get; set; }
    public EmployeeEducationRequest? Education { get; set; }
    public EmployeeTrainingRequest? Training { get; set; }
}
