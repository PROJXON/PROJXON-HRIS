using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSync.Modules.UserManagement.Models;

public class UserRole
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [StringLength(50)]
    public string? Name { get; set; }
    
    [StringLength(250)]
    public string? Description { get; set; }
}