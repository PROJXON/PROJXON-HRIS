using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSync.Modules.EmployeeManagement.Models;

public class SurveyAssignment
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public int SurveyId { get; set; }
    [ForeignKey("SurveyId")]
    public Survey? Survey { get; set; }
    
    public int EmployeeId { get; set; }
    [ForeignKey("EmployeeId")]
    public Employee? Employee { get; set; }
    
    public bool IsCompleted { get; set; }
    public DateTime? AssignedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    
    [Column(TypeName = "jsonb")]
    public string? ResponseJson { get; set; }
}