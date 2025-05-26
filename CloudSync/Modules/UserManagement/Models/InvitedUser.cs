using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Enums.UserManagement;

namespace CloudSync.Modules.UserManagement.Models;

public class InvitedUser
{
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [EmailAddress]
    [StringLength(255)]
    public required string Email { get; set; }
    
    [Required]
    [StringLength(255)]
    public required int InvitedByEmployeeId { get; set; }
    
    [ForeignKey("InvitedByEmployeeId")]
    public User? InvitedByEmployee { get; set; }

    public DateTime CreateDateTime { get; set; } = DateTime.UtcNow;
    
    [Required]
    [StringLength(20)]
    public required string Status { get; set; }
}