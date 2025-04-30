using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSync.Modules.EmployeeManagement.Models;

public class TeamMember
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public int EmployeeId { get; set; }
    
    [ForeignKey("EmployeeId")]
    public Employee? Employee { get; set; }
    
    public int ProjectTeamId { get; set; }
    
    [ForeignKey("ProjectTeamId")]
    public ProjectTeam? ProjectTeam { get; set; }
    
    [StringLength(30)]
    public string? RoleInTeam { get; set; }
}