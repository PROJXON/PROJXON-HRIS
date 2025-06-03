using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSync.Modules.EmployeeManagement.Models;

public class Employee
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public EmployeeBasic BasicInfo { get; set; } = new();
    public EmployeeContactInfo ContactInfo { get; set; } = new();
    
    public int? EmployeeDetailsId { get; set; }
    [ForeignKey("EmployeeDetailsId")]
    public EmployeePosition? PositionDetails { get; set; }
    
    public int? EmployeeDocumentsId { get; set; }
    [ForeignKey("EmployeeDocumentsId")]
    public EmployeeDocuments? Documents { get; set; }
    
    public int? EmployeeLegalId { get; set; }
    [ForeignKey("EmployeeLegalId")]
    public EmployeeLegal? Legal { get; set; }
    
    public int? EmployeeEducationId { get; set; }
    [ForeignKey("EmployeeEducationId")]
    public EmployeeEducation? Education { get; set; }
    
    public int? EmployeeTrainingId { get; set; }
    [ForeignKey("EmployeeTrainingId")]
    public EmployeeTraining? Training{ get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreateDateTime { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdateDateTime { get; set; }
}