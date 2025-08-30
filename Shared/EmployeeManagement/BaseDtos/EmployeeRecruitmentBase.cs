using Shared.EmployeeManagement.Responses;

namespace Shared.EmployeeManagement.BaseDtos;

public class EmployeeRecruitmentBase
{
    public int Id { get; set; }
    public int? ManagerId { get; set; }
    public ManagerOrCoachSummary? RecruitingManager { get; set; }
    public string? RecruitingSource { get; set; }
    public string? InterviewFeedback { get; set; }
    public string? ApplicantStatus { get; set; }
    public string? ContractLength { get; set; }
    public string? OfferDetails { get; set; }
    public string? BackgroundCheckStatus { get; set; }
    public string? TalentPipelineStage { get; set; }
    public string? HireTime { get; set; }
    public string? HireCost { get; set; }
    public bool? InternationalParticipation { get; set; }
 }