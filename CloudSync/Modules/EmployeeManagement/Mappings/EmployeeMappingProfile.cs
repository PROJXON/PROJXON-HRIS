using AutoMapper;
using Shared.EmployeeManagement.Models;
using Shared.EmployeeManagement.Requests;
using Shared.EmployeeManagement.Responses;

namespace CloudSync.Modules.EmployeeManagement.Mappings;

public class EmployeeMappingProfile : Profile
{
    public EmployeeMappingProfile()
    {
        CreateMap<UpdateEmployeeRequest, Employee>()
            .ForMember(dest => dest.CreateDateTime, opt => opt.Ignore())
            .ForMember(dest => dest.UpdateDateTime, opt => opt.Ignore());

        CreateMap<EmployeeDocumentsRequest, EmployeeDocuments>()
            .ForMember(dest => dest.Employee, opt => opt.Ignore());
        CreateMap<EmployeeEducationRequest, EmployeeEducation>()
            .ForMember(dest => dest.Employee, opt => opt.Ignore());
        CreateMap<EmployeeLegalRequest, EmployeeLegal>()
            .ForMember(dest => dest.Employee, opt => opt.Ignore());
        CreateMap<EmployeePositionRequest, EmployeePosition>()
            .ForMember(dest => dest.Employee, opt => opt.Ignore());
        CreateMap<EmployeeTrainingRequest, EmployeeTraining>()
            .ForMember(dest => dest.Employee, opt => opt.Ignore());

        CreateMap<Employee, EmployeeResponse>();

        CreateMap<EmployeeDocuments, EmployeeDocumentsResponse>();
        CreateMap<EmployeeEducation, EmployeeEducationResponse>();
        CreateMap<EmployeeLegal, EmployeeLegalResponse>();
        CreateMap<EmployeePosition, EmployeePositionResponse>();
        CreateMap<EmployeeTraining, EmployeeTrainingResponse>();

        CreateMap<Employee, ManagerOrCoachSummary>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(u => u.Id))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(u => u.BasicInfo.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(u => u.BasicInfo.LastName));
    }
}
