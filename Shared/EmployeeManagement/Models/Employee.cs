using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.EmployeeManagement.Models;

public class Employee
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    // Owned
    public EmployeeBasic BasicInfo { get; set; } = new();
    public EmployeeContactInfo ContactInfo { get; set; } = new();

    public EmployeePosition? PositionDetails { get; set; }
    public EmployeeDocuments? Documents { get; set; }
    public EmployeeLegal? Legal { get; set; }
    public EmployeeEducation? Education { get; set; }
    public EmployeeTraining? Training { get; set; }

    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
}
