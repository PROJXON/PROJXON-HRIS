using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.EmployeeManagement.Models;

public class Employee
{
    public static Employee CreateDefault()
    {
        var employee = new Employee();
        
        employee.PositionDetails = new EmployeePosition {Employee = employee };
        employee.Documents = new EmployeeDocuments { Employee = employee };
        employee.Legal = new EmployeeLegal { Employee = employee };
        employee.Education = new EmployeeEducation { Employee = employee };
        employee.Training = new EmployeeTraining { Employee = employee };
        
        return employee;
    }
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    // Owned
    public EmployeeBasic BasicInfo { get; set; } = new();
    public EmployeeContactInfo ContactInfo { get; set; } = new();

    public EmployeePosition? PositionDetails { get; set; }
    public EmployeeDocuments? Documents { get; set; }
    public EmployeeLegal? Legal { get; set; }
    public EmployeeEducation? Education { get; set; }
    public EmployeeTraining? Training{ get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreateDateTime { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdateDateTime { get; set; }
}