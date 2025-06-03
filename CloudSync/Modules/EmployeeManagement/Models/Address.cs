using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSync.Modules.EmployeeManagement.Models;

public class Address
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [StringLength(80)]
    public required string AddressLine1 { get; set; }
    
    [StringLength(80)]
    public string? AddressLine2 { get; set; }
    
    [Required]
    [StringLength(50)]
    public required string Country { get; set; }
    
    [Required]
    [StringLength(50)]
    public required string StateOrProvince { get; set; }
    
    [Required]
    [StringLength(50)]
    public required string City { get; set; }
    
    [Required]
    [StringLength(15)]
    public required string PostalCode { get; set; }
    
    [StringLength(30)]
    public string? TimeZone { get; set; }
}