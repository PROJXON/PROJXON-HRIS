using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.UserManagement.Models;

public class Permission
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [StringLength(40)]
    public string? Name { get; set; }
    
    [StringLength(250)]
    public string? Description { get; set; }
}