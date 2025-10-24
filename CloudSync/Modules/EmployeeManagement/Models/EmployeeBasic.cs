using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSync.Modules.EmployeeManagement.Models;

public class EmployeeBasic
{
    [StringLength(40)] public string? FirstName { get; set; }
    [StringLength(40)] public string? LastName { get; set; }
    [StringLength(40)] public string? PreferredName { get; set; }
    [StringLength(40)] public string? NamePronunciation { get; set; }
    public DateTime? DateOfBirth { get; set; }
    [StringLength(40)] public string? Gender { get; set; }
    [StringLength(20)] public string? MaritalStatus { get; set; }
    [StringLength(40)] public string? Nationality { get; set; }
    [StringLength(40)] public string? Race { get; set; }
    [StringLength(40)] public string? Ethnicity { get; set; }
    [StringLength(40)] public string? PreferredPronouns { get; set; }
}