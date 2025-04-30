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
    public string? Username { get; set; }
    
    [Required]
    [StringLength(40)]
    public string? Password { get; set; }
    
    [Required]
    public int RoleId { get; set; } 
    
    [ForeignKey("RoleId")]
    public UserRole? Role { get; set; }
    
    [Required]
    public int EmployeeId { get; set; }
    
    [ForeignKey("EmployeeId")]
    public Employee? Employee { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreateDateTime { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime LastLoginDateTime { get; set; }
    
    [Column(TypeName = "jsonb")]
    public string? UserSettings { get; set; }
}