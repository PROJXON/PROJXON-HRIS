using AutoMapper;
using CloudSync.Modules.EmployeeManagement.Models;
using Shared.EmployeeManagement.Dtos;
using Shared.EmployeeManagement.Requests;
using Shared.EmployeeManagement.Responses;

namespace CloudSync.Modules.EmployeeManagement.Mappings;

public class EmployeeMappingProfile : Profile
{
    public EmployeeMappingProfile()
    {
        CreateMap<CreateEmployeeRequest, Employee>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProjxonEmail, opt => opt.Ignore())
            .ForMember(dest => dest.Phone, opt => opt.Ignore())
            .ForMember(dest => dest.Address, opt => opt.Ignore())
            .ForMember(dest => dest.PersonalEmail, opt => opt.Ignore())
            .ForMember(dest => dest.EmergencyContactName, opt => opt.Ignore())
            .ForMember(dest => dest.EmergencyContactPhone, opt => opt.Ignore())
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
            .ForMember(dest => dest.OnboardingDate, opt => opt.Ignore())
            .ForMember(dest => dest.OffboardingDate, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreateDateTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdateDateTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.ProfilePictureUrl, opt => opt.Ignore())
            .ForMember(dest => dest.ResumeUrl, opt => opt.Ignore())
            .ForMember(dest => dest.CoverLetterUrl, opt => opt.Ignore())
            .ForMember(dest => dest.LinkedInUrl, opt => opt.Ignore())
            .ForMember(dest => dest.GitHubUrl, opt => opt.Ignore())
            .ForMember(dest => dest.PersonalWebsiteUrl, opt => opt.Ignore())
            .ForMember(dest => dest.WorkAuthorizationType, opt => opt.Ignore())
            .ForMember(dest => dest.WorkAuthorizationDocumentUrl, opt => opt.Ignore())
            .ForMember(dest => dest.VisaNumber, opt => opt.Ignore())
            .ForMember(dest => dest.VisaExpirationDate, opt => opt.Ignore())
            .ForMember(dest => dest.WorkPermitNumber, opt => opt.Ignore())
            .ForMember(dest => dest.WorkExpirationDate, opt => opt.Ignore())
            .ForMember(dest => dest.EducationLevel, opt => opt.Ignore())
            .ForMember(dest => dest.UniversitiesAttended, opt => opt.Ignore())
            .ForMember(dest => dest.DegreesEarned, opt => opt.Ignore())
            .ForMember(dest => dest.TimeZone, opt => opt.Ignore())
            .ForMember(dest => dest.City, opt => opt.Ignore())
            .ForMember(dest => dest.State, opt => opt.Ignore())
            .ForMember(dest => dest.Country, opt => opt.Ignore())
            .ForMember(dest => dest.RecruitingSource, opt => opt.Ignore())
            .ForMember(dest => dest.CanvasCoursesCompleted, opt => opt.Ignore())
            .ForMember(dest => dest.CanvasCertificates, opt => opt.Ignore())
            .ForMember(dest => dest.NewCompany, opt => opt.Ignore())
            .ForMember(dest => dest.OnboardingChecklist, opt => opt.Ignore());
        
        CreateMap<UpdateEmployeeRequest, EmployeeDto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PositionId, opt => opt.Ignore())
            .ForMember(dest => dest.ManagerId, opt => opt.Ignore())
            .ForMember(dest => dest.CoachId, opt => opt.Ignore())
            .ForMember(dest => dest.CreateDateTime, opt => opt.Ignore())
            .ForMember(dest => dest.UpdateDateTime, opt => opt.Ignore());
        
        CreateMap<EmployeeDto, Employee>()
            .ForMember(dest => dest.Position, opt => opt.Ignore())
            .ForMember(dest => dest.Manager, opt => opt.Ignore())
            .ForMember(dest => dest.Coach, opt => opt.Ignore());
        
        CreateMap<Employee, EmployeeDto>();
        CreateMap<EmployeeDto, EmployeeResponse>();
    }
}