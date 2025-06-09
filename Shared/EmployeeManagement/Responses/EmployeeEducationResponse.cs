namespace Shared.EmployeeManagement.Responses;

public class EmployeeEducationResponse
{
    public int Id { get; set; }
    public string? EducationLevel { get; set; }
    public List<string>? UniversitiesAttended { get; set; }
    public List<string>? DegreesEarned { get; set; }
}