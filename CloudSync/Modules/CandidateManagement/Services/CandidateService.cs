using AutoMapper;
using CloudSync.Exceptions.Business;
using CloudSync.Infrastructure;
using CloudSync.Modules.CandidateManagement.Models;
using CloudSync.Modules.CandidateManagement.Repositories;
using CloudSync.Modules.EmployeeManagement.Models;
using CloudSync.Modules.EmployeeManagement.Repositories.Interfaces;
using CloudSync.Modules.UserManagement.Services.Interfaces;
using Shared.Requests.UserManagement;
using Microsoft.EntityFrameworkCore;

namespace CloudSync.Modules.CandidateManagement.Services;

public interface ICandidateService
{
    Task<IEnumerable<Candidate>> GetAllAsync();
    Task<Candidate> CreateAsync(Candidate candidate);
    Task HireCandidateAsync(int candidateId);
    Task UpdateStatusAsync(int id, string status);
}

public class CandidateService(
    ICandidateRepository candidateRepo,
    IEmployeeRepository employeeRepo,
    IInvitedUserService invitedUserService,
    DatabaseContext dbContext,
    IMapper mapper) : ICandidateService
{
    public async Task<IEnumerable<Candidate>> GetAllAsync() => await candidateRepo.GetAllAsync();

    public async Task<Candidate> CreateAsync(Candidate candidate) => await candidateRepo.AddAsync(candidate);

    public async Task HireCandidateAsync(int candidateId)
    {
        var candidate = await candidateRepo.GetByIdAsync(candidateId);
        if (candidate == null) throw new EntityNotFoundException("Candidate not found");

        // Check Status (Fast check)
        if (string.Equals(candidate.Status, "Hired", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        // Check for Existing Employee by Email (Robust check)
        var existingEmployee = await dbContext.Employees
            .Where(e => e.ContactInfo.PersonalEmail == candidate.Email)
            .FirstOrDefaultAsync();

        if (existingEmployee != null)
        {
            // Employee exists, just ensure candidate status is updated and return
            candidate.Status = "Hired";
            candidate.OnboardingDate = DateTime.UtcNow;
            await candidateRepo.UpdateAsync(candidate);
            return;
        }

        // Create Employee Record (Only if not found above)
        var newEmployee = new Employee
        {
            BasicInfo = new EmployeeBasic
            {
                FirstName = candidate.FirstName,
                LastName = candidate.LastName,
            },
            ContactInfo = new EmployeeContactInfo
            {
                PersonalEmail = candidate.Email,
                PhoneNumber = candidate.Phone
            },
            PositionDetails = new EmployeePosition 
            { 
                PositionName = candidate.JobAppliedFor,
                OnboardingDate = DateTime.UtcNow,
                EmploymentStatus = Shared.EmployeeManagement.Enums.EmployeeStatus.Active
            },
            CreateDateTime = DateTime.UtcNow,
            UpdateDateTime = DateTime.UtcNow
        };

        await employeeRepo.CreateAsync(newEmployee);

        // Create Invite (Automated)
        if (!string.IsNullOrEmpty(candidate.Email))
        {
            var systemInviter = await dbContext.Users
                .Where(u => u.RoleId == 1 || u.RoleId == 2)
                .FirstOrDefaultAsync();

            int inviterId = systemInviter?.Id ?? 1;

            await invitedUserService.InviteUserAsync(new InvitedUserRequest
            {
                Email = candidate.Email,
                InvitedByEmployeeId = inviterId
            });
        }

        // Update Candidate Status
        candidate.Status = "Hired";
        candidate.OnboardingDate = DateTime.UtcNow;
        await candidateRepo.UpdateAsync(candidate);
    }

    public async Task UpdateStatusAsync(int id, string status)
    {
        var candidate = await candidateRepo.GetByIdAsync(id);
        if (candidate == null) return;
        
        candidate.Status = status;
        await candidateRepo.UpdateAsync(candidate);
    }
}