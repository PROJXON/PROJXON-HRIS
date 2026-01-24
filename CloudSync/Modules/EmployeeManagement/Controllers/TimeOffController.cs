using CloudSync.Infrastructure;
using CloudSync.Modules.EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.TimeOffManagement.Requests;
using Shared.TimeOffManagement.Responses;
using System.Security.Claims;

namespace CloudSync.Modules.EmployeeManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
// TODO: Re-enable [Authorize] once authentication is properly configured
public class TimeOffController(DatabaseContext context) : ControllerBase
{
    /// <summary>
    /// Get all time off requests for the current intern
    /// </summary>
    [HttpGet("my-requests")]
    public async Task<ActionResult<IEnumerable<TimeOffRequestResponse>>> GetMyRequests()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        // TODO: Remove this temporary fix once authentication is working
        if (string.IsNullOrEmpty(userId))
        {
            // For testing: return all requests
            var allRequests = await context.TimeOffRequests
                .Include(r => r.Employee)
                .OrderByDescending(r => r.SubmittedDate)
                .ToListAsync();
            return Ok(allRequests.Select(MapToResponse));
        }

        var requests = await context.TimeOffRequests
            .Include(r => r.Employee)
            .Where(r => r.EmployeeId.ToString() == userId)
            .OrderByDescending(r => r.SubmittedDate)
            .ToListAsync();

        return Ok(requests.Select(MapToResponse));
    }

    /// <summary>
    /// Get time off balance for the current intern
    /// </summary>
    [HttpGet("balance")]
    public async Task<ActionResult<TimeOffBalanceResponse>> GetBalance()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int employeeId = 1; // Default for testing
        
        if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out var parsedId))
        {
            employeeId = parsedId;
        }

        var requests = await context.TimeOffRequests
            .Where(r => r.EmployeeId == employeeId)
            .ToListAsync();

        var totalDaysAvailable = 10; // Default PTO days per year
        var daysUsed = requests.Where(r => r.Status == "Approved").Sum(r => r.TotalDays);
        var daysPending = requests.Where(r => r.Status == "Pending").Sum(r => r.TotalDays);
        var daysRemaining = totalDaysAvailable - daysUsed - daysPending;

        var balance = new TimeOffBalanceResponse
        {
            TotalDaysAvailable = totalDaysAvailable,
            DaysUsed = daysUsed,
            DaysPending = daysPending,
            DaysRemaining = daysRemaining
        };

        return Ok(balance);
    }

    /// <summary>
    /// Create a new time off request
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TimeOffRequestResponse>> CreateRequest([FromBody] CreateTimeOffRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int employeeId = 1; // Default for testing
        
        if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out var parsedId))
        {
            employeeId = parsedId;
        }

        // Validate request
        if (request.StartDate > request.EndDate)
            return BadRequest("End date must be after start date");

        if (string.IsNullOrWhiteSpace(request.Reason))
            return BadRequest("Reason is required");

        // Check balance
        var requests = await context.TimeOffRequests
            .Where(r => r.EmployeeId == employeeId)
            .ToListAsync();

        var totalDaysAvailable = 10;
        var daysUsed = requests.Where(r => r.Status == "Approved").Sum(r => r.TotalDays);
        var daysPending = requests.Where(r => r.Status == "Pending").Sum(r => r.TotalDays);
        var daysRemaining = totalDaysAvailable - daysUsed - daysPending;

        if (request.TotalDays > daysRemaining)
            return BadRequest("Insufficient time off balance");

        // Get employee info
        var employee = await context.Employees.FindAsync(employeeId);
        if (employee == null)
            return NotFound("Employee not found");

        // Create the request
        var timeOffRequest = new TimeOffRequest
        {
            EmployeeId = employeeId,
            StartDate = request.StartDate.ToUniversalTime(),
            EndDate = request.EndDate.ToUniversalTime(),
            RequestType = request.RequestType,
            Reason = request.Reason,
            Status = "Pending",
            SubmittedDate = DateTime.UtcNow,
            TotalDays = request.TotalDays
        };

        context.TimeOffRequests.Add(timeOffRequest);
        await context.SaveChangesAsync();

        return Ok(MapToResponse(timeOffRequest));
    }

    /// <summary>
    /// Cancel a pending time off request
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> CancelRequest(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        if (!int.TryParse(userId, out var employeeId))
            return Unauthorized();

        var request = await context.TimeOffRequests
            .FirstOrDefaultAsync(r => r.Id == id && r.EmployeeId == employeeId);

        if (request == null)
            return NotFound("Request not found");

        if (request.Status != "Pending")
            return BadRequest("Only pending requests can be cancelled");

        context.TimeOffRequests.Remove(request);
        await context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Get all time off requests (for HR users)
    /// </summary>
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<TimeOffRequestResponse>>> GetAllRequests()
    {
        var requests = await context.TimeOffRequests
            .Include(r => r.Employee)
            .OrderByDescending(r => r.SubmittedDate)
            .ToListAsync();

        return Ok(requests.Select(MapToResponse));
    }

    /// <summary>
    /// Approve or deny a time off request (for HR users)
    /// </summary>
    [HttpPut("{id}/review")]
    public async Task<ActionResult<TimeOffRequestResponse>> ReviewRequest(int id, [FromBody] ReviewTimeOffRequest review)
    {
        var reviewerName = GetCurrentUserName();
        if (reviewerName == null)
            return Unauthorized();

        var request = await context.TimeOffRequests
            .Include(r => r.Employee)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null)
            return NotFound("Request not found");

        if (request.Status != "Pending")
            return BadRequest("Only pending requests can be reviewed");

        // Update status based on Decision field from ReviewTimeOffRequest
        request.Status = review.Decision; // "Approved" or "Denied"
        request.ReviewedDate = DateTime.UtcNow;
        request.ReviewedBy = reviewerName;

        await context.SaveChangesAsync();

        return Ok(MapToResponse(request));
    }

    #region Helper Methods

    private string? GetCurrentUserName()
    {
        var firstName = User.FindFirstValue(ClaimTypes.GivenName);
        var lastName = User.FindFirstValue(ClaimTypes.Surname);
        
        if (!string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
            return $"{firstName} {lastName}";
        
        return User.FindFirstValue(ClaimTypes.Name);
    }

    private static TimeOffRequestResponse MapToResponse(TimeOffRequest r) => new()
    {
        Id = r.Id,
        InternId = r.EmployeeId,
        InternName = r.Employee != null 
            ? $"{r.Employee.BasicInfo.FirstName} {r.Employee.BasicInfo.LastName}".Trim()
            : string.Empty,
        StartDate = r.StartDate,
        EndDate = r.EndDate,
        RequestType = r.RequestType,
        Reason = r.Reason,
        Status = r.Status,
        SubmittedDate = r.SubmittedDate,
        ReviewedDate = r.ReviewedDate,
        ReviewedBy = r.ReviewedBy,
        TotalDays = r.TotalDays
    };

    #endregion
}