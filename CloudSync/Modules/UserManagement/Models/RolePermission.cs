using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSync.Modules.UserManagement.Models;

public class RolePermission
{
    [Required]
    public int UserRoleId { get; set; }
    
    [ForeignKey("RoleId")]
    public UserRole? UserRole { get; set; }
    
    [Required]
    public int PermissionId { get; set; }
    
    [ForeignKey("EmployeeId")]
    public Permission? Permission { get; set; }
}