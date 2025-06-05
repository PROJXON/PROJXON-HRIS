using AutoMapper;
using CloudSync.Modules.EmployeeManagement.Models;
using Shared.EmployeeManagement.Dtos;
using Shared.EmployeeManagement.Requests;
using Shared.EmployeeManagement.Responses;
using Shared.Enums.UserManagement;

namespace CloudSync.Modules.EmployeeManagement.Mappings;

public class EmployeeMappingProfile : Profile
{
    public EmployeeMappingProfile()
    {
        CreateMap<CreateEmployeeRequest, Employee>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreateDateTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdateDateTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.BasicInfo, opt => opt.Ignore())
            .ForMember(dest => dest.ContactInfo, opt => opt.Ignore())
            .ForMember(dest => dest.Documents, opt => opt.Ignore())
            .ForMember(dest => dest.Education, opt => opt.Ignore())
            .ForMember(dest => dest.Legal, opt => opt.Ignore())
            .ForMember(dest => dest.Training, opt => opt.Ignore())
            .ForMember(dest => dest.PositionDetails, opt => opt.Ignore())
            .ForPath(dest => dest.BasicInfo.FirstName, opt => opt.MapFrom(u => u.FirstName))
            .ForPath(dest => dest.BasicInfo.LastName, opt => opt.MapFrom(u => u.LastName));
        
        CreateMap<UpdateEmployeeRequest, EmployeeDto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreateDateTime, opt => opt.Ignore())
            .ForMember(dest => dest.UpdateDateTime, opt => opt.Ignore())
            .ForMember(dest => dest.PositionSummary, opt => opt.Ignore())
            .ForMember(dest => dest.ManagerSummary, opt => opt.Ignore())
            .ForMember(dest => dest.CoachSummary, opt => opt.Ignore());

        CreateMap<EmployeeDto, Employee>();

        CreateMap<Employee, EmployeeDto>();

        CreateMap<EmployeeDto, EmployeeResponse>();

        CreateMap<Employee, ManagerOrCoachSummary>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(u => u.Id));
        
        CreateMap<EmployeePosition, PositionSummary>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(u => u.Id))
            .ForMember(dest => dest.PositionName, opt => opt.MapFrom(u => u.PositionName))
            .ForMember(dest => dest.HierarchyLevel, opt => opt.MapFrom(u => Enum.Parse<HierarchyLevel>(u.HierarchyLevel)));
    }
}