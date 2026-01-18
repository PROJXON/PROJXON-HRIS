using System;
using System.Threading;
using System.Threading.Tasks;
using Client.Utils.Classes;
using Client.Utils.Interfaces;

namespace Client.Services;

/// <summary>
/// Service for managing user invitations.
/// </summary>
public interface IInvitationService
{
    /// <summary>
    /// Sends an invitation to the specified email address.
    /// </summary>
    /// <param name="email">The email address to invite.</param>
    /// <param name="invitedByEmployeeId">The ID of the employee sending the invitation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>API response indicating success or failure.</returns>
    Task<ApiResponse<InvitationResponse>> SendInvitationAsync(
        string email, 
        int invitedByEmployeeId, 
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Response model for invitation creation.
/// </summary>
public class InvitationResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreateDateTime { get; set; }
}

/// <summary>
/// Request model for creating an invitation.
/// </summary>
public class CreateInvitationRequest
{
    public string Email { get; set; } = string.Empty;
    public int InvitedByEmployeeId { get; set; }
}

/// <summary>
/// Implementation of the invitation service using the API client.
/// </summary>
public class InvitationService(IApiClient apiClient) : IInvitationService
{
    private const string InvitedUserEndpoint = "api/InvitedUser";

    public async Task<ApiResponse<InvitationResponse>> SendInvitationAsync(
        string email, 
        int invitedByEmployeeId, 
        CancellationToken cancellationToken = default)
    {
        var request = new CreateInvitationRequest
        {
            Email = email,
            InvitedByEmployeeId = invitedByEmployeeId
        };

        return await apiClient.PostAsync<InvitationResponse>(
            InvitedUserEndpoint, 
            request, 
            cancellationToken);
    }
}