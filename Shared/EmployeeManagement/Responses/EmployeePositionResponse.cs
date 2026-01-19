using Shared.EmployeeManagement.BaseDtos;

namespace Shared.EmployeeManagement.Responses;

/// <summary>
/// Response model for employee position/job details.
/// </summary>
public class EmployeePositionResponse : EmployeePositionBase
{
    // Properties mapped specifically for the frontend view
    public string? JobTitle { get; set; }
    public string? Department { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? WorkLocation { get; set; }
    public decimal? Salary { get; set; }
    public string? PayFrequency { get; set; }
}