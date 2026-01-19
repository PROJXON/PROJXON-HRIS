namespace Shared.Attendance;

public class AttendanceResponse
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public double HoursWorked { get; set; }
}