using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSync.Modules.EmployeeManagement.Models;

public class EmployeeEducation
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public Employee? Employee { get; set; }

    [StringLength(20)]
    public string? EducationLevel { get; set; }

    [StringLength(100)]
    public string? UndergraduateSchool { get; set; }

    [StringLength(100)]
    public string? UndergraduateDegree { get; set; }

    [StringLength(100)]
    public string? GraduateSchool { get; set; }

    [StringLength(100)]
    public string? GraduateDegree { get; set; }

    [Column(TypeName = "jsonb")]
    public List<string>? UniversitiesAttended { get; set; }

    [Column(TypeName = "jsonb")]
    public List<string>? DegreesEarned { get; set; }
}
