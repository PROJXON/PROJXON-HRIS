namespace Shared.EmployeeManagement.BaseDtos;

public class EmployeeLegalBase
{
    public int Id { get; set; }
    public string? PersonalIdNumber { get; set; }
    public string? PersonalIdType { get; set; }
    public string? PersonalIdCountry { get; set; }
    public string? PersonalIdState { get; set; }
    public string? WorkAuthorizationType { get; set; }
    public string? WorkAuthorizationDocumentUrl { get; set; }
    public string? VisaNumber { get; set; }
    public DateTime? VisaExpirationDate { get; set; }
    public string? WorkPermitNumber { get; set; }
    public DateTime? WorkExpirationDate { get; set; }
    public string? LaborLawCompliance { get; set; }
    public string? EmployeeContracts { get; set; }
    public string? Agreements { get; set; }
    public string? EeoReporting { get; set; }
    public string? DataPrivacyConsents { get; set; }
    public string? DisciplinaryRecords { get; set; }
    public string? PolicyAcknowledgements { get; set; }
}
