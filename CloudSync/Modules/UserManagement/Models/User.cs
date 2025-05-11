using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CloudSync.Modules.EmployeeManagement.Models;

namespace CloudSync.Modules.UserManagement.Models;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [EmailAddress]
    [StringLength(40)]
    public string Username { get; set; } = string.Empty;

    [Required] 
    [StringLength(60)] 
    public required string Password { get; set; }
    
    // public int RoleId { get; set; } 
    //
    // [ForeignKey("RoleId")]
    // public UserRole? Role { get; set; }
    //
    // public int EmployeeId { get; set; }
    //
    // [ForeignKey("EmployeeId")]
    // public Employee? Employee { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreateDateTime { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime LastLoginDateTime { get; set; }
    
    [Column(TypeName = "jsonb")]
    public string? UserSettings { get; set; }
}