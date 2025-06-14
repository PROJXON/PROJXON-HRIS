﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Shared.EmployeeManagement.Models;

[Owned]
public class EmployeeContactInfo
{
    [Phone]
    [StringLength(20)]
    public string? Phone { get; set; }
    
    [StringLength(30)]
    public string? InternationalPhone { get; set; }
    
    [StringLength(30)]
    public string? InternationalPhoneType { get; set; }
    
    public int? AddressId { get; set; }
    
    [ForeignKey("AddressId")]
    public Address? Address { get; set; }
    
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