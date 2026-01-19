using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CloudSync.Modules.EmployeeManagement.Models;

[Owned]
public class EmployeeContactInfo
{
    [EmailAddress] [StringLength(60)] public string? PersonalEmail { get; set; }
    [Phone] [StringLength(20)] public string? PhoneNumber { get; set; }
    [StringLength(30)] public string? InternationalPhoneNumber { get; set; }
    [StringLength(30)] public string? InternationalPhoneType { get; set; }
    public Address PermanentAddress { get; set; } = new();
    public Address MailingAddress { get; set; } = new();
    [StringLength(40)] public string? EmergencyContactName { get; set; }
    [Phone][StringLength(15)] public string? EmergencyContactPhoneNumber { get; set; }
    [EmailAddress] [StringLength(60)] public string? ProjxonEmail { get; set; }
    
    [StringLength(50)] public string? DiscordUsername { get; set; }
}