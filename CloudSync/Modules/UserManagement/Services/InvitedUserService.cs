using AutoMapper;
using CloudSync.Exceptions.Business;
using CloudSync.Modules.UserManagement.Repositories.Interfaces;
using CloudSync.Modules.UserManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Requests.UserManagement;
using Shared.Responses.UserManagement;

namespace CloudSync.Modules.UserManagement.Services;

public class InvitedUserService (IInvitedUserRepository invitedUserRepository, IMapper mapper) : IInvitedUserService
{
    public async Task<IEnumerable<InvitedUserResponse>> GetAllAsync()
    {
        var invitedUserList = await invitedUserRepository.GetAllAsync();
        return invitedUserList.Select(mapper.Map<InvitedUserResponse>);
    }
    
    public async Task<InvitedUserResponse> InviteUserAsync(InvitedUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ValidationException("Email is required.");
        
        var existingInvite = await invitedUserRepository.GetByEmailAsync(request.Email);
        if (existingInvite != null)
        {
            throw new DuplicateEntityException("Email has already been invited.");
        }

        var newInvitedUser = await invitedUserRepository.AddAsync(request);

        return mapper.Map<InvitedUserResponse>(newInvitedUser);
    }

    public async Task DeleteInviteAsync(int id)
    {
        var wasDeleted = await invitedUserRepository.DeleteAsync(id);
        if (!wasDeleted)
            throw new EntityNotFoundException("No invite exists for this user.");
    }
}