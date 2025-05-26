using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CloudSync.Modules.EmployeeManagement.Models;

namespace CloudSync.Modules.UserManagement.Models;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string GoogleUserId { get; set; }
    
    [Required]
    [EmailAddress]
    [StringLength(40)]
    public required string Email { get; set; }
    
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
    public DateTime CreateDateTime { get; set; } = DateTime.UtcNow;
    
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime LastLoginDateTime { get; set; }
    
    public bool IsActive { get; set; }
    
    [Column(TypeName = "jsonb")]
    public string? UserSettings { get; set; }
}