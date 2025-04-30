using System.ComponentModel.DataAnnotations;

namespace CloudSync.Modules.EmployeeManagement.Models;
using System.ComponentModel.DataAnnotations.Schema;

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