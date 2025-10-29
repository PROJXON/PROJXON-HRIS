namespace Shared.EmployeeManagement.BaseDtos;

public class EmployeeEducationBase
{
    public int Id { get; set; }
    public string? EducationLevel { get; set; }
    public string? UndergraduateSchool { get; set; }
    public string? UndergraduateDegree { get; set; }
    public string? GraduateSchool { get; set; }
    public string? GraduateDegree { get; set; }
    public List<string>? UniversitiesAttended { get; set; }
    public List<string>? DegreesEarned { get; set; }
}
