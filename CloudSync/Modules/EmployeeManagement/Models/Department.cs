using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSync.Modules.EmployeeManagement.Models;

public class Department
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [StringLength(50)]
    public string? Name { get; set; }
    
    public int? ParentDepartmentId { get; set; } 
    
    [ForeignKey("ParentDepartmentId")]
    public Department? ParentDepartment { get; set; }
}