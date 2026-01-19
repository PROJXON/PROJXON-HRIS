namespace Shared.EmployeeManagement.Responses;

public class SurveyAssignmentResponse
{
    public int Id { get; set; }
    public int SurveyId { get; set; }
    public int EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public DateTime? AssignedDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? ResponseJson { get; set; }
    public SurveyResponse? Survey { get; set; }
}