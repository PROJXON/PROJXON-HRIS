using AutoMapper;
using CloudSync.Modules.UserManagement.Models;
using Shared.DTOs.UserManagement;
using Shared.Requests.UserManagement;
using Shared.Responses.UserManagement;

namespace CloudSync.Modules.UserManagement.Mappings;

public class InvitedUserMappingProfile : Profile
{
    public InvitedUserMappingProfile()
    {
        CreateMap<InvitedUserRequest, InvitedUserDto>();
        CreateMap<InvitedUserDto, InvitedUser>();
        CreateMap<InvitedUser, InvitedUserResponse>();
    }
}