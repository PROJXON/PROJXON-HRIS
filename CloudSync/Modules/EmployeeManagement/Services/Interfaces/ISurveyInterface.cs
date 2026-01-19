using Shared.EmployeeManagement.Requests;
using Shared.EmployeeManagement.Responses;

namespace CloudSync.Modules.EmployeeManagement.Services.Interfaces;

public interface ISurveyService
{
    Task<IEnumerable<SurveyResponse>> GetAllSurveysAsync();
    Task<IEnumerable<SurveyAssignmentResponse>> GetAllAssignmentsAsync(); // NEW
    Task<SurveyResponse?> GetSurveyByIdAsync(int id);
    Task<SurveyAssignmentResponse?> GetAssignmentByIdAsync(int assignmentId);
    Task<SurveyResponse> CreateSurveyAsync(CreateSurveyRequest request);
    Task AssignSurveyToAllEmployeesAsync(int surveyId);
    Task<IEnumerable<TaskItemResponse>> GetTasksForEmployeeAsync(int employeeId);
    Task CompleteSurveyAsync(int assignmentId, string responseJson);
}