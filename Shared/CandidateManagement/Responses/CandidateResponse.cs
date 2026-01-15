namespace Shared.CandidateManagement.Responses;

public class CandidateResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string JobAppliedFor { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty; // Added to match frontend needs
    public string Status { get; set; } = string.Empty;
    public DateTime AppliedDate { get; set; }
}