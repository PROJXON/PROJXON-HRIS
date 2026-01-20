namespace Shared.EmployeeManagement.Requests;

public class CreateSurveyRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string QuestionsJson { get; set; } = string.Empty;
}