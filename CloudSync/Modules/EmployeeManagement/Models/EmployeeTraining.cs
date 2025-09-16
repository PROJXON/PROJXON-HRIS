using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSync.Modules.EmployeeManagement.Models;

public class EmployeeTraining
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public Employee? Employee { get; set; }

    [Column(TypeName = "jsonb")]
    public List<string>? CanvasCertificates { get; set; }
    
    [Column(TypeName = "jsonb")]
    public List<string>? CanvasCoursesCompleted { get; set; }

    [StringLength(40)]
    public string? DevelopmentPlan { get; set; }

    public int? TrainingHoursLogged { get; set; }

    [Column(TypeName = "jsonb")]
    public List<string>? Evaluation { get; set; }

    public bool? LeadershipParticipation { get; set; }

    [Url]
    [StringLength(200)]
    public string? ParticipantDashboard { get; set; }

    [Url]
    [StringLength(200)]
    public string? ParticipantMeetingLink { get; set; }
    
    [StringLength(40)]
    public string? NewCompany { get; set; }
    
    [Column(TypeName = "jsonb")]
    public List<string>? OnboardingChecklist { get; set; }
}