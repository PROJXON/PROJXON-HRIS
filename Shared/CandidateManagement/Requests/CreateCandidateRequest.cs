namespace Shared.CandidateManagement.Requests;

public class CreateCandidateRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string JobAppliedFor { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}