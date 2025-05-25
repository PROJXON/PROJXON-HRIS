using CloudSync.Modules.UserManagement.Repositories.Interfaces;
using CloudSync.Modules.UserManagement.Services.Exceptions;
using CloudSync.Modules.UserManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.UserManagement;
using Shared.Enums.UserManagement;
using Shared.Requests.UserManagement;
using Shared.Responses.UserManagement;

namespace CloudSync.Modules.UserManagement.Services;

public class InvitedUserService (IInvitedUserRepository invitedUserRepository) : IInvitedUserService
{
    public async Task<IEnumerable<InviteUserResponse>> GetAllAsync()
    {
        var invitedUserList = await invitedUserRepository.GetAllAsync();
        List<InviteUserResponse> invitedUserResponseList = [];
            
        invitedUserResponseList.AddRange(invitedUserList.Select(invitedUser => new InviteUserResponse
        {
            Id = invitedUser.Id,
            Email = invitedUser.Email,
            InvitedByEmployeeId = invitedUser.InvitedByEmployeeId,
            Status = Enum.Parse<InvitedUserStatus>(invitedUser.Status),
            CreateDateTime = invitedUser.CreateDateTime,
        }));

        return invitedUserResponseList;
    }
    
    public async Task<ActionResult<InviteUserResponse>> InviteUserAsync(InviteUserRequest request)
    {
        var existingInvite = await invitedUserRepository.GetByEmail(request.Email);
        if (existingInvite != null)
        {
            throw new InvitedUserException("Email has already been invited.", 409);
        }
        
        var invitedUserDto = new InvitedUserDto
        {
            Email = request.Email,
            InvitedByEmployeeId = request.InvitedByEmployeeId,
            Status = nameof(InvitedUserStatus.Pending)
        };

        var newInvitedUser = await invitedUserRepository.Add(invitedUserDto);
        
        return new InviteUserResponse
        {
            Id = newInvitedUser.Id,
            Email = newInvitedUser.Email,
            InvitedByEmployeeId = newInvitedUser.InvitedByEmployeeId,
            Status = Enum.Parse<InvitedUserStatus>(newInvitedUser.Status),
            CreateDateTime = newInvitedUser.CreateDateTime
        };
    }

    public async Task DeleteInviteAsync(int id)
    {
        await invitedUserRepository.DeleteAsync(id);
    }
}