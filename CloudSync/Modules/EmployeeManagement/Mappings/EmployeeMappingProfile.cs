using AutoMapper;
using CloudSync.Modules.EmployeeManagement.Models;
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

        CreateMap<EmployeeBasicRequest, EmployeeBasic>();
        CreateMap<EmployeeContactInfoRequest, EmployeeContactInfo>();
        CreateMap<AddressRequest, Address>();
        CreateMap<EmployeeDocumentsRequest, EmployeeDocuments>()
            .ForMember(dest => dest.Employee, opt => opt.Ignore());
        CreateMap<EmployeeEducationRequest, EmployeeEducation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Employee, opt => opt.Ignore());
        CreateMap<EmployeeLegalRequest, EmployeeLegal>()
            .ForMember(dest => dest.Employee, opt => opt.Ignore());
        CreateMap<EmployeePositionRequest, EmployeePosition>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Employee, opt => opt.Ignore())
            .ForMember(dest => dest.Department, opt => opt.Ignore())
            .ForMember(dest => dest.SubDepartment, opt => opt.Ignore())
            .ForMember(dest => dest.Manager, opt => opt.Ignore())
            .ForMember(dest => dest.Coach, opt => opt.Ignore())
            .ForMember(dest => dest.ManagerId, opt => opt.MapFrom(u => u.Manager != null ? u.Manager.Id : (int?)null))
            .ForMember(dest => dest.CoachId, opt => opt.MapFrom(u => u.Coach != null ? u.Coach.Id : (int?)null));
        CreateMap<EmployeeRecruitmentRequest, EmployeeRecruitment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Employee, opt => opt.Ignore())
            .ForMember(dest => dest.RecruitingManager, opt => opt.Ignore())
            .ForMember(dest => dest.ManagerId, opt => opt.MapFrom(u => u.RecruitingManager != null ? u.RecruitingManager.Id : (int?)null));
        CreateMap<EmployeeTrainingRequest, EmployeeTraining>()
            .ForMember(dest => dest.Employee, opt => opt.Ignore());

        CreateMap<Employee, EmployeeResponse>();

        CreateMap<EmployeeBasic, EmployeeBasicResponse>();
        CreateMap<EmployeeContactInfo, EmployeeContactInfoResponse>();
        CreateMap<Address, AddressResponse>();
        CreateMap<EmployeeDocuments, EmployeeDocumentsResponse>();
        CreateMap<EmployeeEducation, EmployeeEducationResponse>();
        CreateMap<EmployeeLegal, EmployeeLegalResponse>();
        CreateMap<EmployeePosition, EmployeePositionResponse>();
        CreateMap<EmployeeRecruitment, EmployeeRecruitmentResponse>();
        CreateMap<EmployeeTraining, EmployeeTrainingResponse>();

        CreateMap<Employee, ManagerOrCoachSummary>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(u => u.Id))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(u => u.BasicInfo.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(u => u.BasicInfo.LastName));
    }
}
