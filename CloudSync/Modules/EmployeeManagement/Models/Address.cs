using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CloudSync.Modules.EmployeeManagement.Models;

[Owned]
public class Address
{
    [StringLength(80)]
    public string? AddressLine1 { get; set; }
    
    [StringLength(80)]
    public string? AddressLine2 { get; set; }
    
    [StringLength(50)]
    public string? Country { get; set; }
    
    [StringLength(50)]
    public string? StateOrProvince { get; set; }
    
    [StringLength(50)]
    public string? City { get; set; }
    
    [StringLength(15)]
    public string? PostalCode { get; set; }
    
    [StringLength(30)]
    public string? TimeZone { get; set; }
}