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
        CreateMap<InvitedUser, InvitedUserResponse>();
        CreateMap<InvitedUserRequest, InvitedUser>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Pending"))
            .ForMember(dest => dest.CreateDateTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.InvitedByEmployee, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}