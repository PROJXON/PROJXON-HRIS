using System;

namespace Shared.TimeOffManagement.Responses;

/// <summary>
/// Response containing time off request details
/// </summary>
public class TimeOffRequestResponse
{
    public int Id { get; set; }
    public int InternId { get; set; }
    public string InternName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string RequestType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, Approved, Denied
    public DateTime SubmittedDate { get; set; }
    public DateTime? ReviewedDate { get; set; }
    public string ReviewedBy { get; set; } = string.Empty;
    public int TotalDays { get; set; }
}

/// <summary>
/// Response containing intern's time off balance
/// </summary>
public class TimeOffBalanceResponse
{
    public int InternId { get; set; }
    public int TotalDaysAvailable { get; set; }
    public int DaysUsed { get; set; }
    public int DaysPending { get; set; }
    public int DaysRemaining { get; set; }
}