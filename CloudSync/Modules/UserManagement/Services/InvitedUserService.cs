using AutoMapper;
using CloudSync.Modules.UserManagement.Repositories.Interfaces;
using CloudSync.Modules.UserManagement.Services.Exceptions;
using CloudSync.Modules.UserManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.UserManagement;
using Shared.Enums.UserManagement;
using Shared.Requests.UserManagement;
using Shared.Responses.UserManagement;

namespace CloudSync.Modules.UserManagement.Services;

public class InvitedUserService (IInvitedUserRepository invitedUserRepository, IMapper mapper) : IInvitedUserService
{
    public async Task<IEnumerable<InvitedUserResponse>> GetAllAsync()
    {
        var invitedUserList = await invitedUserRepository.GetAllAsync();
        List<InvitedUserResponse> invitedUserResponseList = [];
            
        invitedUserResponseList.AddRange(invitedUserList.Select(invitedUser => new InvitedUserResponse
        {
            Id = invitedUser.Id,
            Email = invitedUser.Email,
            InvitedByEmployeeId = invitedUser.InvitedByEmployeeId,
            Status = Enum.Parse<InvitedUserStatus>(invitedUser.Status),
            CreateDateTime = invitedUser.CreateDateTime,
        }));

        return invitedUserResponseList;
    }
    
    public async Task<ActionResult<InvitedUserResponse>> InviteUserAsync(InvitedUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new InvitedUserException("Email is required.", 400);
        
        var existingInvite = await invitedUserRepository.GetByEmailAsync(request.Email);
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

        var newInvitedUser = await invitedUserRepository.AddAsync(invitedUserDto);
        
        return new InvitedUserResponse
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