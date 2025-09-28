namespace Shared.EmployeeManagement.BaseDtos;

public class EmployeeTrainingBase
{
    public int Id { get; set; }
    public List<string>? CanvasCoursesCompleted { get; set; }
    public List<string>? CanvasCertificates { get; set; }
    public string? NewCompany { get; set; }
    public List<string>? OnboardingChecklist { get; set; }
    public string? DevelopmentPlan { get; set; }
    public int? TrainingHoursLogged { get; set; }
    public List<string>? Evaluation { get; set; }
    public bool? LeadershipParticipation { get; set; }
    public string? ParticipantDashboard { get; set; }
    public string? ParticipantMeetingLink { get; set; }
}
