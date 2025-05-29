using AutoMapper;
using CloudSync.Modules.EmployeeManagement.Models;
using Shared.EmployeeManagement.Requests;
using Shared.EmployeeManagement.Responses;

namespace CloudSync.Modules.EmployeeManagement.Mappings;

public class EmployeeMappingProfile : Profile
{
    public EmployeeMappingProfile()
    {
        CreateMap<Employee, EmployeeResponse>();
        CreateMap<CreateEmployeeRequest, Employee>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProjxonEmail, opt => opt.Ignore())
            .ForMember(dest => dest.Phone, opt => opt.Ignore())
            .ForMember(dest => dest.Address, opt => opt.Ignore())
            .ForMember(dest => dest.PersonalEmail, opt => opt.Ignore())
            .ForMember(dest => dest.EmergencyContactName, opt => opt.Ignore())
            .ForMember(dest => dest.EmergencyContactPhone, opt => opt.Ignore())
            .ForMember(dest => dest.SocialSecurityNumber, opt => opt.Ignore())
            .ForMember(dest => dest.IdNumber, opt => opt.Ignore())
            .ForMember(dest => dest.IdType, opt => opt.Ignore())
            .ForMember(dest => dest.IdCountry, opt => opt.Ignore())
            .ForMember(dest => dest.IdState, opt => opt.Ignore())
            .ForMember(dest => dest.BirthCountry, opt => opt.Ignore())
            .ForMember(dest => dest.BirthState, opt => opt.Ignore())
            .ForMember(dest => dest.BirthCity, opt => opt.Ignore())
            .ForMember(dest => dest.BirthDate, opt => opt.Ignore())
            .ForMember(dest => dest.Gender, opt => opt.Ignore())
            .ForMember(dest => dest.PreferredPronouns, opt => opt.Ignore())
            .ForMember(dest => dest.PreferredName, opt => opt.Ignore())
            .ForMember(dest => dest.PositionId, opt => opt.Ignore())
            .ForMember(dest => dest.Position, opt => opt.Ignore())
            .ForMember(dest => dest.ManagerId, opt => opt.Ignore())
            .ForMember(dest => dest.Manager, opt => opt.Ignore())
            .ForMember(dest => dest.CoachId, opt => opt.Ignore())
            .ForMember(dest => dest.Coach, opt => opt.Ignore())
            .ForMember(dest => dest.CreateDateTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
            ;
    }
}