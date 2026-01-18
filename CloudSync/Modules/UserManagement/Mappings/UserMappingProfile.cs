using AutoMapper;
using Shared.Responses.UserManagement;
using CloudSync.Modules.UserManagement.Models;
using Shared.UserManagement.Requests;

namespace CloudSync.Modules.UserManagement.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UpdateUserRequest, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.GoogleUserId, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.Employee, opt => opt.Ignore())
            .ForMember(dest => dest.CreateDateTime, opt => opt.Ignore())
            .ForMember(dest => dest.LastLoginDateTime, opt => opt.Ignore())
            .ForMember(dest => dest.RoleId, opt => opt.Ignore())
            .ForMember(dest => dest.EmployeeId, opt => opt.Ignore());
            
        CreateMap<User, UserResponse>();
    }
}