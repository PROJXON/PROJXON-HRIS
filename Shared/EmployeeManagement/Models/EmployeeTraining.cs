using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.EmployeeManagement.Models;

public class EmployeeTraining
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public Employee Employee { get; set; }
    
    [Column(TypeName = "jsonb")]
    public List<string>? CanvasCoursesCompleted { get; set; }
    
    [Column(TypeName = "jsonb")]
    public List<string>? CanvasCertificates { get; set; }
    
    [StringLength(40)]
    public string? NewCompany { get; set; }
    
    [Column(TypeName = "jsonb")]
    public List<string>? OnboardingChecklist { get; set; }
}