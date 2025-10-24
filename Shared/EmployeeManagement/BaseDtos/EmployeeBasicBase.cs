namespace Shared.EmployeeManagement.BaseDtos;

public class EmployeeBasicBase
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PreferredName { get; set; }
    public string? NamePronunciation { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? MaritalStatus { get; set; }
    public string? Nationality { get; set; }
    public string? Race { get; set; }
    public string? Ethnicity { get; set; }
    public string? PreferredPronouns { get; set; }
}