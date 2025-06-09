namespace Shared.EmployeeManagement.Responses;

public class EmployeeLegalResponse
{
    public int Id { get; set; }
    public string? IdNumber { get; set; }
    public string? IdType { get; set; }
    public string? IdCountry { get; set; }
    public string? IdState { get; set; }
    public string? WorkAuthorizationType { get; set; }
    public string? WorkAuthorizationDocumentUrl { get; set; }
    public string? VisaNumber { get; set; }
    public DateTime? VisaExpirationDate { get; set; }
    public string? WorkPermitNumber { get; set; }
    public DateTime? WorkExpirationDate { get; set; }
}