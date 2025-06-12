namespace Shared.EmployeeManagement.BaseDtos;

public class EmployeeTrainingBase
{
    public int Id { get; set; }
    public List<string>? CanvasCoursesCompleted { get; set; }
    public List<string>? CanvasCertificates { get; set; }
    public string? NewCompany { get; set; }
    public List<string>? OnboardingChecklist { get; set; }
}
