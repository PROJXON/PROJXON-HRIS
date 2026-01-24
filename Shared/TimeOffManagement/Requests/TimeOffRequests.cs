using System;

namespace Shared.TimeOffManagement.Requests;

/// <summary>
/// Request to create a new time off request
/// </summary>
public class CreateTimeOffRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string RequestType { get; set; } = string.Empty; // Vacation, Sick Leave, Personal Day, etc.
    public string Reason { get; set; } = string.Empty;
    public int TotalDays { get; set; }
}

/// <summary>
/// Request to review (approve/deny) a time off request
/// </summary>
public class ReviewTimeOffRequest
{
    public string Decision { get; set; } = string.Empty; // "Approved" or "Denied"
    public string ReviewNotes { get; set; } = string.Empty;
}

/// <summary>
/// Request to update time off balance
/// </summary>
public class UpdateTimeOffBalanceRequest
{
    public int InternId { get; set; }
    public int TotalDaysAvailable { get; set; }
}