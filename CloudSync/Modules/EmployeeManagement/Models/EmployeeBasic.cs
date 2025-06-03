using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSync.Modules.EmployeeManagement.Models;

public class EmployeeBasic
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [StringLength(40)]
    public string? FirstName { get; set; }
    
    [StringLength(40)]
    public string? LastName { get; set; }
    
    [StringLength(40)] 
    public string? Nationality { get; set; }

    [StringLength(40)]
    public string? Ethnicity { get; set; }
    
    [StringLength(40)]
    public string? BirthCountry { get; set; }
    
    [StringLength(40)]
    public string? BirthState { get; set; }
    
    [StringLength(40)]
    public string? BirthCity { get; set; }
    
    public DateTime? BirthDate { get; set; }
    
    [StringLength(40)]
    public string? Gender { get; set; }
    
    [StringLength(20)]
    public string? MaritalStatus { get; set; }
    
    [StringLength(40)]
    public string? PreferredPronouns { get; set; }
    
    [StringLength(40)]
    public string? PreferredName { get; set; }
    
    [StringLength(40)]
    public string? NickName { get; set; }
}