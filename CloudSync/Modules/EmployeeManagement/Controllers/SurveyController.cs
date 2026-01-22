using Microsoft.AspNetCore.Mvc;
using CloudSync.Modules.EmployeeManagement.Services.Interfaces;
using Shared.EmployeeManagement.Requests;
using Shared.EmployeeManagement.Responses;

namespace CloudSync.Modules.EmployeeManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SurveyController(ISurveyService surveyService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SurveyResponse>>> GetAll()
    {
        var result = await surveyService.GetAllSurveysAsync();
        return Ok(result);
    }
    
    [HttpGet("assignments")]
    public async Task<ActionResult<IEnumerable<SurveyAssignmentResponse>>> GetAllAssignments()
    {
        var result = await surveyService.GetAllAssignmentsAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SurveyResponse>> GetById(int id)
    {
        var result = await surveyService.GetSurveyByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("{id:int}/assignments")]
    public async Task<ActionResult<IEnumerable<SurveyAssignmentResponse>>> GetSurveyAssignments(int id)
    {
        var result = await surveyService.GetAssignmentsBySurveyIdAsync(id);
        return Ok(result);
    }

    [HttpGet("assignment/{assignmentId:int}")]
    public async Task<ActionResult<SurveyAssignmentResponse>> GetAssignment(int assignmentId)
    {
        var assignment = await surveyService.GetAssignmentByIdAsync(assignmentId);
        if (assignment == null) return NotFound();
        return Ok(assignment);
    }

    [HttpPost]
    public async Task<ActionResult<SurveyResponse>> CreateSurvey([FromBody] CreateSurveyRequest request)
    {
        var result = await surveyService.CreateSurveyAsync(request);
        return Ok(result);
    }

    [HttpPost("{surveyId:int}/assign-all")]
    public async Task<IActionResult> AssignToAll(int surveyId)
    {
        await surveyService.AssignSurveyToAllEmployeesAsync(surveyId);
        return Ok(new { message = "Survey assigned to all employees" });
    }

    [HttpGet("tasks/{employeeId:int}")]
    public async Task<ActionResult<IEnumerable<TaskItemResponse>>> GetEmployeeTasks(int employeeId)
    {
        var tasks = await surveyService.GetTasksForEmployeeAsync(employeeId);
        return Ok(tasks);
    }

    [HttpPost("complete/{assignmentId:int}")]
    public async Task<IActionResult> CompleteSurvey(int assignmentId, [FromBody] object answers)
    {
        await surveyService.CompleteSurveyAsync(assignmentId, answers.ToString() ?? "{}");
        return Ok(new { message = "Survey completed" });
    }
}