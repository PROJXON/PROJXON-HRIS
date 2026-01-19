using CloudSync.Infrastructure;
using CloudSync.Modules.EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Attendance;

namespace CloudSync.Modules.EmployeeManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AttendanceController(DatabaseContext context) : ControllerBase
{
    [HttpGet("{employeeId:int}")]
    public async Task<ActionResult<IEnumerable<AttendanceResponse>>> GetEmployeeAttendance(int employeeId)
    {
        var records = await context.Attendance
            .Where(a => a.EmployeeId == employeeId)
            .OrderByDescending(a => a.Date)
            .ToListAsync();

        return Ok(records.Select(MapToResponse));
    }
    
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<AttendanceResponse>>> GetAllAttendance()
    {
        var records = await context.Attendance.ToListAsync();
        return Ok(records.Select(MapToResponse));
    }

    [HttpPost]
    public async Task<ActionResult<AttendanceResponse>> SubmitAttendance([FromBody] CreateAttendanceRequest request)
    {
        // Check for existing entry for this day to update instead of duplicate
        var dateOnly = request.Date.Date.ToUniversalTime(); // Ensure UTC for comparison
        
        var existing = await context.Attendance
            .FirstOrDefaultAsync(a => a.EmployeeId == request.EmployeeId && a.Date.Date == dateOnly.Date);

        if (existing != null)
        {
            existing.StartTime = request.StartTime;
            existing.EndTime = request.EndTime;
            await context.SaveChangesAsync();
            return Ok(MapToResponse(existing));
        }

        var attendance = new Attendance
        {
            EmployeeId = request.EmployeeId,
            Date = dateOnly,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };

        context.Attendance.Add(attendance);
        await context.SaveChangesAsync();

        return Ok(MapToResponse(attendance));
    }

    private static AttendanceResponse MapToResponse(Attendance a) => new()
    {
        Id = a.Id,
        EmployeeId = a.EmployeeId,
        Date = a.Date,
        StartTime = a.StartTime,
        EndTime = a.EndTime,
        HoursWorked = (a.EndTime - a.StartTime).TotalHours
    };
}