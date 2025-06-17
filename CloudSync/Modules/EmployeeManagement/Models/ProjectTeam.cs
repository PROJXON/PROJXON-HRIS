using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CloudSync.Modules.EmployeeManagement.Models;

public class ProjectTeam
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [StringLength(30)]
    public string? Name { get; set; }
    
    [StringLength(250)]
    public string? Description { get; set; }
}