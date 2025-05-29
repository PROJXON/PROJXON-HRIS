using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Enums.UserManagement;

namespace CloudSync.Modules.EmployeeManagement.Models;

public class Position
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public required string PositionName { get; set; }
    
    [Required]
    public int DepartmentId { get; set; }
    [ForeignKey("DepartmentId")]
    public required Department Department { get; set; }
    
    [Required]
    public int SubDepartmentId { get; set; }
    [ForeignKey("SubDepartmentId")]
    public required Department SubDepartment { get; set; }
    
    [Required]
    [StringLength(50)]
    public required string HierarchyLevel { get; set; }}