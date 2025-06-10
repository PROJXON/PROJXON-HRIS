namespace Shared.EmployeeManagement.BaseDtos;

public class EmployeeEducationBase
{
    public string? EducationLevel { get; set; }
    public List<string>? UniversitiesAttended { get; set; }
    public List<string>? DegreesEarned { get; set; }
}