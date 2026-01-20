namespace Shared.EmployeeManagement.Responses;

public class TaskItemResponse
{
    public int Id { get; set; } 
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = "Task"; 
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedDate { get; set; }
    public int ReferenceId { get; set; }

}