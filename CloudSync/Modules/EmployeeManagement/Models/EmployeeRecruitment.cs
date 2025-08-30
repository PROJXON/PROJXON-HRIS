using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSync.Modules.EmployeeManagement.Models;

public class EmployeeRecruitment
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public Employee? Employee { get; set; }

    public int? ManagerId { get; set; }
    [ForeignKey("ManagerId")]
    public Employee? RecruitingManager { get; set; }

    [StringLength(30)]
    public string? RecruitingSource { get; set; }

    [StringLength(500)]
    public string? InterviewFeedback { get; set; }

    [StringLength(20)]
    public string? ApplicantStatus { get; set; }

    [StringLength(20)]
    public string? ContractLength { get; set; }

    [StringLength(200)]
    public string? OfferDetails { get; set; }

    [StringLength(30)]
    public string? BackgroundCheckStatus { get; set; }

    [StringLength(30)]
    public string? TalentPipelineStage { get; set; }

    [StringLength(20)]
    public string? HireTime { get; set; }

    [StringLength(20)]
    public string? HireCost { get; set; }

    public bool? InternationalParticipation { get; set; }
 }