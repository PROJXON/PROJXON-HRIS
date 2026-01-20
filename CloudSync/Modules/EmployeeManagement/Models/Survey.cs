using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSync.Modules.EmployeeManagement.Models;

public class Survey
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [Column(TypeName = "jsonb")]
    public string QuestionsJson { get; set; } = "[]";
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public int CreatedByHrId { get; set; } // HR Employee ID
    
    public bool IsActive { get; set; }
}