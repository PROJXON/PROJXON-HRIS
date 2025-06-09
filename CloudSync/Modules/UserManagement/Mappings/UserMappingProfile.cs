using AutoMapper;
using Shared.Responses.UserManagement;
using Shared.UserManagement.Models;
using Shared.UserManagement.Requests;

namespace CloudSync.Modules.UserManagement.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UpdateUserRequest, User>()
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.Employee, opt => opt.Ignore())
            .ForMember(dest => dest.CreateDateTime, opt => opt.Ignore())
            .ForMember(dest => dest.LastLoginDateTime, opt => opt.Ignore());
        CreateMap<User, UserResponse>();
    }
}