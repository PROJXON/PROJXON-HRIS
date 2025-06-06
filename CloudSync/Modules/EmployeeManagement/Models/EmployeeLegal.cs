using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSync.Modules.EmployeeManagement.Models;

public class EmployeeLegal
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public required Employee Employee { get; set; }
    
    [StringLength(40)]
    public string? IdNumber { get; set; }
    
    [StringLength(40)]
    public string? IdType { get; set; }
    
    [StringLength(40)]
    public string? IdCountry { get; set; }
    
    [StringLength(40)]
    public string? IdState { get; set; }
    
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
}