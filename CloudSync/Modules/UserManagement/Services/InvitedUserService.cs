using AutoMapper;
using CloudSync.Modules.UserManagement.Repositories.Interfaces;
using CloudSync.Modules.UserManagement.Services.Exceptions;
using CloudSync.Modules.UserManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
            
        invitedUserResponseList.AddRange(invitedUserList.Select(mapper.Map<InvitedUserResponse>));

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

        var newInvitedUser = await invitedUserRepository.AddAsync(request);

        return mapper.Map<InvitedUserResponse>(newInvitedUser);
    }

    public async Task DeleteInviteAsync(int id)
    {
        await invitedUserRepository.DeleteAsync(id);
    }
}