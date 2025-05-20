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
    public string? Email { get; set; }
    
    [Required]
    public int InvitedById { get; set; }
    
    [ForeignKey("InvitedById")]
    public User? InvitedBy { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreateDateTime { get; set; }
    
    [Required]
    [StringLength(20)]
    public required InvitedUserStatus Status { get; set; }
}