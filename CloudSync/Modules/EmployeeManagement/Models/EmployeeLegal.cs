using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSync.Modules.EmployeeManagement.Models;

public class EmployeeLegal
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public Employee? Employee { get; set; }

    [StringLength(40)]
    public string? PersonalIdNumber { get; set; }

    [StringLength(40)]
    public string? PersonalIdType { get; set; }

    [StringLength(40)]
    public string? PersonalIdCountry { get; set; }

    [StringLength(40)]
    public string? PersonalIdState { get; set; }

    [StringLength(20)]
    public string? WorkAuthorizationType { get; set; }

    [Url]
    [StringLength(200)]
    public string? WorkAuthorizationDocumentUrl { get; set; }

    [StringLength(20)]
    public string? VisaNumber { get; set; }

    public DateTime? VisaExpirationDate { get; set; }

    [StringLength(20)]
    public string? WorkPermitNumber { get; set; }

    public DateTime? WorkExpirationDate { get; set; }

    [StringLength(20)]
    public string? LaborLawCompliance { get; set; }

    [Url]
    [StringLength(200)]
    public string? EmployeeContracts { get; set; }

    [Url]
    [StringLength(200)]
    public string? Agreements { get; set; }

    [Url]
    [StringLength(200)]
    public string? EeoReporting { get; set; }

    [Url]
    [StringLength(200)]
    public string? DataPrivacyConsents { get; set; }

    [Url]
    [StringLength(200)]
    public string? DisciplinaryRecords { get; set; }

    [Url]
    [StringLength(200)]
    public string? PolicyAcknowledgements { get; set; }
}