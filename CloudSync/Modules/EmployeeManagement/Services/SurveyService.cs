using CloudSync.Exceptions.Business; 
using CloudSync.Infrastructure;
using CloudSync.Modules.EmployeeManagement.Models;
using CloudSync.Modules.EmployeeManagement.Repositories.Interfaces;
using CloudSync.Modules.EmployeeManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.EmployeeManagement.Requests;
using Shared.EmployeeManagement.Responses;

namespace CloudSync.Modules.EmployeeManagement.Services;

public class SurveyService(
    DatabaseContext _context, 
    IEmployeeRepository _employeeRepo) : ISurveyService
{
    public async Task<IEnumerable<SurveyResponse>> GetAllSurveysAsync()
    {
        return await _context.Surveys
            .OrderByDescending(s => s.CreatedDate)
            .Select(s => new SurveyResponse
            {
                Id = s.Id,
                Title = s.Title,
                Description = s.Description,
                QuestionsJson = s.QuestionsJson,
                IsActive = s.IsActive,
                CreatedDate = s.CreatedDate,
                CreatedByHrId = s.CreatedByHrId
            })
            .ToListAsync();
    }

    public async Task<SurveyResponse?> GetSurveyByIdAsync(int id)
    {
        var s = await _context.Surveys.FindAsync(id);
        if (s == null) return null;

        return new SurveyResponse
        {
            Id = s.Id,
            Title = s.Title,
            Description = s.Description,
            QuestionsJson = s.QuestionsJson,
            IsActive = s.IsActive,
            CreatedDate = s.CreatedDate,
            CreatedByHrId = s.CreatedByHrId
        };
    }

    public async Task<SurveyAssignmentResponse?> GetAssignmentByIdAsync(int assignmentId)
    {
        var assignment = await _context.SurveyAssignments
            .Include(sa => sa.Survey)
            .FirstOrDefaultAsync(sa => sa.Id == assignmentId);
        
        if (assignment == null) return null;
        
        return new SurveyAssignmentResponse
        {
            Id = assignment.Id,
            SurveyId = assignment.SurveyId,
            EmployeeId = assignment.EmployeeId,
            AssignedDate = assignment.AssignedDate,
            IsCompleted = assignment.IsCompleted,
            CompletedDate = assignment.CompletedDate,
            ResponseJson = assignment.ResponseJson,
            Survey = assignment.Survey == null ? null : new SurveyResponse
            {
                Id = assignment.Survey.Id,
                Title = assignment.Survey.Title,
                Description = assignment.Survey.Description,
                QuestionsJson = assignment.Survey.QuestionsJson,
                IsActive = assignment.Survey.IsActive,
                CreatedDate = assignment.Survey.CreatedDate,
                CreatedByHrId = assignment.Survey.CreatedByHrId
            }
        };
    }

    // NEW METHOD: Get all assignments for HR dashboard
    public async Task<IEnumerable<SurveyAssignmentResponse>> GetAllAssignmentsAsync()
    {
        return await _context.SurveyAssignments
            .Include(sa => sa.Survey)
            .Include(sa => sa.Employee)
            .OrderByDescending(sa => sa.CompletedDate)
            .Select(sa => new SurveyAssignmentResponse
            {
                Id = sa.Id,
                SurveyId = sa.SurveyId,
                EmployeeId = sa.EmployeeId,
                EmployeeName = sa.Employee != null ? $"{sa.Employee.BasicInfo.FirstName} {sa.Employee.BasicInfo.LastName}" : "Unknown",
                AssignedDate = sa.AssignedDate,
                IsCompleted = sa.IsCompleted,
                CompletedDate = sa.CompletedDate,
                ResponseJson = sa.ResponseJson,
                Survey = sa.Survey == null ? null : new SurveyResponse
                {
                    Id = sa.Survey.Id,
                    Title = sa.Survey.Title
                }
            })
            .ToListAsync();
    }

    // Get assignments for a specific survey
    public async Task<IEnumerable<SurveyAssignmentResponse>> GetAssignmentsBySurveyIdAsync(int surveyId)
    {
        // This executes "WHERE survey_id = X" in SQL
        return await _context.SurveyAssignments
            .Include(sa => sa.Employee) // Load employee to get names
            .Where(sa => sa.SurveyId == surveyId)
            .OrderByDescending(sa => sa.CompletedDate)
            .Select(sa => new SurveyAssignmentResponse
            {
                Id = sa.Id,
                SurveyId = sa.SurveyId,
                EmployeeId = sa.EmployeeId,
                // Handle null employee gracefully
                EmployeeName = sa.Employee != null 
                    ? $"{sa.Employee.BasicInfo.FirstName} {sa.Employee.BasicInfo.LastName}" 
                    : "Unknown",
                AssignedDate = sa.AssignedDate,
                IsCompleted = sa.IsCompleted,
                CompletedDate = sa.CompletedDate,
                // We usually don't need the full JSON list for the summary view, 
                // but we include it if needed for the ViewModel logic
                ResponseJson = sa.ResponseJson 
            })
            .ToListAsync();
    }

    public async Task<SurveyResponse> CreateSurveyAsync(CreateSurveyRequest request)
    {
        var survey = new Survey
        {
            Title = request.Title,
            Description = request.Description,
            QuestionsJson = request.QuestionsJson,
            IsActive = false, 
            CreatedDate = DateTime.UtcNow,
            CreatedByHrId = 1 // Mock HR ID for now
        };

        await _context.Surveys.AddAsync(survey);
        await _context.SaveChangesAsync();

        return new SurveyResponse
        {
            Id = survey.Id,
            Title = survey.Title,
            Description = survey.Description,
            QuestionsJson = survey.QuestionsJson,
            IsActive = survey.IsActive,
            CreatedDate = survey.CreatedDate,
            CreatedByHrId = survey.CreatedByHrId
        };
    }

    public async Task AssignSurveyToAllEmployeesAsync(int surveyId)
    {
        var employees = await _employeeRepo.GetAllAsync();
        
        var assignments = employees.Select(e => new SurveyAssignment
        {
            SurveyId = surveyId,
            EmployeeId = e.Id,
            AssignedDate = DateTime.UtcNow,
            IsCompleted = false
        });
        
        await _context.SurveyAssignments.AddRangeAsync(assignments);
        
        // Update Survey status to Active
        var survey = await _context.Surveys.FindAsync(surveyId);
        if(survey != null) 
        {
            survey.IsActive = true;
        }
        
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TaskItemResponse>> GetTasksForEmployeeAsync(int employeeId)
    {
        return await _context.SurveyAssignments
            .Include(sa => sa.Survey)
            .Where(sa => sa.EmployeeId == employeeId)
            .Select(sa => new TaskItemResponse 
            {
                Id = sa.Id, // Important: This is the Assignment ID, not Survey ID
                Title = sa.Survey!.Title,
                Type = "Survey",
                ReferenceId = sa.SurveyId,
                DueDate = sa.AssignedDate.Value.AddDays(7), 
                IsCompleted = sa.IsCompleted,
                CompletedDate = sa.CompletedDate
            })
            .ToListAsync();
    }

    public async Task CompleteSurveyAsync(int assignmentId, string responseJson)
    {
        var assignment = await _context.SurveyAssignments.FindAsync(assignmentId);
        
        if(assignment == null) 
            throw new EntityNotFoundException("Task assignment not found");
        
        assignment.IsCompleted = true;
        assignment.CompletedDate = DateTime.UtcNow;
        assignment.ResponseJson = responseJson;
        
        await _context.SaveChangesAsync();
    }
}