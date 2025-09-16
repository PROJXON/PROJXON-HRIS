using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSync.Modules.EmployeeManagement.Models;

public class EmployeeTechnicalSpec
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public Employee? Employee { get; set; }

    [StringLength(20)]
    public string? LaptopModel { get; set; }

    [StringLength(20)]
    public string? OperatingSystem { get; set; }

    [StringLength(20)]
    public string? CellphoneModel { get; set; }
}