namespace Shared.Attendance;

public class CreateAttendanceRequest
{
    public int EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}