using System;
using System.ComponentModel.DataAnnotations;

namespace CloudSync.Modules.EmployeeManagement.Models;

/// <summary>
/// Represents a time off request submitted by an employee/intern
/// </summary>
public class TimeOffRequest
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    /// <summary>
    /// Navigation property to the employee who submitted the request
    /// </summary>
    public Employee? Employee { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    [MaxLength(50)]
    public string RequestType { get; set; } = string.Empty; // Vacation, Sick Leave, Personal Day, etc.

    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending"; // Pending, Approved, Denied

    [Required]
    public DateTime SubmittedDate { get; set; }

    public DateTime? ReviewedDate { get; set; }

    [MaxLength(100)]
    public string ReviewedBy { get; set; } = string.Empty;

    [Required]
    public int TotalDays { get; set; }
}