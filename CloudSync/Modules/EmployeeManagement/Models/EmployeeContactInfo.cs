using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CloudSync.Modules.EmployeeManagement.Models;

[Owned]
public class EmployeeContactInfo
{
    [Phone]
    [StringLength(20)]
    public string? Phone { get; set; }
    
    public Address Address { get; set; } = new();
    
    [StringLength(30)]
    public string? InternationalPhone { get; set; }
    
    [StringLength(30)]
    public string? InternationalPhoneType { get; set; }
    
    [EmailAddress]
    [StringLength(60)]
    public string? ProjxonEmail { get; set; }
    
    [EmailAddress]
    [StringLength(60)]
    public string? PersonalEmail { get; set; }
    
    [StringLength(40)]
    public string? EmergencyContactName { get; set; }
    
    [Phone]
    [StringLength(15)]
    public string? EmergencyContactPhone { get; set; }
}