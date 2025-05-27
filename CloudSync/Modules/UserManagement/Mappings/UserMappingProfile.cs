using AutoMapper;
using CloudSync.Modules.UserManagement.Models;
using Shared.Responses.UserManagement;

namespace CloudSync.Modules.UserManagement.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserResponse>();
    }
}