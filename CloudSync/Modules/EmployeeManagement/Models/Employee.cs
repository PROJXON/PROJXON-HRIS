using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSync.Modules.EmployeeManagement.Models;

public class Employee
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public EmployeeBasic BasicInfo { get; set; } = new();
    public EmployeeContactInfo ContactInfo { get; set; } = new();
    public EmployeePosition PositionDetails { get; set; } = new();
    public EmployeeDocuments Documents { get; set; } = new();
    public EmployeeLegal Legal { get; set; } = new();
    public EmployeeEducation Education { get; set; } = new();
    public EmployeeTraining Training{ get; set; } = new();
    public EmployeeMetadata Metadata { get; set; } = new();
}