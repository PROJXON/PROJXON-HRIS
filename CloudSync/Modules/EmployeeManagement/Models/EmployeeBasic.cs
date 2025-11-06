using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Enums.UserManagement;

namespace CloudSync.Modules.EmployeeManagement.Models;

public class EmployeeBasic
{
    [StringLength(40)] public string? FirstName { get; set; }
    [StringLength(40)] public string? LastName { get; set; }
    [StringLength(40)] public string? PreferredName { get; set; }
    [StringLength(40)] public string? NamePronunciation { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }
    public MaritalStatus? MaritalStatus { get; set; }
    [StringLength(40)] public string? Nationality { get; set; }
    [StringLength(40)] public string? Race { get; set; }
    [StringLength(40)] public string? Ethnicity { get; set; }
    [StringLength(40)] public string? PreferredPronouns { get; set; }
    public DateTime? TimeZone { get; set; }
}