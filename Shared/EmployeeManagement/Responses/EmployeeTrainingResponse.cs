namespace Shared.EmployeeManagement.Responses;

public class EmployeeTrainingResponse
{
    public List<string>? CanvasCoursesCompleted { get; set; }
    public List<string>? CanvasCertificates { get; set; }
    public string? NewCompany { get; set; }
    public List<string>? OnboardingChecklist { get; set; }
}