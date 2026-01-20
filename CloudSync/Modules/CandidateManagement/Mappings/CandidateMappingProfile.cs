using AutoMapper;
using CloudSync.Modules.CandidateManagement.Models;
using Shared.CandidateManagement.Requests;
using Shared.CandidateManagement.Responses;

namespace CloudSync.Modules.CandidateManagement.Mappings;

public class CandidateMappingProfile : Profile
{
    public CandidateMappingProfile()
    {
        // Request -> Database Entity
        CreateMap<CreateCandidateRequest, Candidate>()
            .ForMember(dest => dest.CreateDateTime, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdateDateTime, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Applied"))
            
            // Location Splitting Logic
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => 
                !string.IsNullOrEmpty(src.Location) && src.Location.Contains(',') 
                    ? src.Location.Split(',', StringSplitOptions.TrimEntries)[0] 
                    : src.Location))
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => 
                !string.IsNullOrEmpty(src.Location) && src.Location.Contains(',') 
                    ? src.Location.Split(',', StringSplitOptions.TrimEntries)[1] 
                    : string.Empty))

            // IGNORE LIST - Start
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AvailabilityDate, opt => opt.Ignore())
            .ForMember(dest => dest.OnboardingDate, opt => opt.Ignore())
            .ForMember(dest => dest.ResumeUrl, opt => opt.Ignore())
            .ForMember(dest => dest.CoverLetterUrl, opt => opt.Ignore())
            .ForMember(dest => dest.LinkedInUrl, opt => opt.Ignore())
            .ForMember(dest => dest.GitHubUrl, opt => opt.Ignore())
            .ForMember(dest => dest.PersonalWebsiteUrl, opt => opt.Ignore())
            .ForMember(dest => dest.WorkAuthorizationType, opt => opt.Ignore())
            .ForMember(dest => dest.EducationLevel, opt => opt.Ignore())
            .ForMember(dest => dest.UniversitiesAttended, opt => opt.Ignore())
            .ForMember(dest => dest.TimeZone, opt => opt.Ignore())
            .ForMember(dest => dest.RecruitingSource, opt => opt.Ignore())
            
            // --- FIXED: Explicitly ignore InterviewerId ---
            .ForMember(dest => dest.InterviewerId, opt => opt.Ignore()) 
            
            .ForMember(dest => dest.Interviewer, opt => opt.Ignore())
            .ForMember(dest => dest.Interviewers, opt => opt.Ignore())
            .ForMember(dest => dest.InterviewDateTime, opt => opt.Ignore())
            .ForMember(dest => dest.Notes, opt => opt.Ignore());
            // IGNORE LIST - End

        // Database Entity -> Response
        CreateMap<Candidate, CandidateResponse>()
            .ForMember(dest => dest.AppliedDate, opt => opt.MapFrom(src => src.CreateDateTime))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => 
                string.IsNullOrEmpty(src.State) ? src.City : $"{src.City}, {src.State}"));
    }
}