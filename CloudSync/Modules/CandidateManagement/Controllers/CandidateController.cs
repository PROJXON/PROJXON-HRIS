using CloudSync.Modules.CandidateManagement.Services;
using CloudSync.Modules.CandidateManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.CandidateManagement.Requests;
using Shared.CandidateManagement.Responses;
using AutoMapper;

namespace CloudSync.Modules.CandidateManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CandidateController(ICandidateService candidateService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CandidateResponse>>> GetAll()
    {
        var candidates = await candidateService.GetAllAsync();
        return Ok(mapper.Map<IEnumerable<CandidateResponse>>(candidates));
    }

    [HttpPost]
    public async Task<ActionResult<CandidateResponse>> Create([FromBody] CreateCandidateRequest request)
    {
        var candidateModel = mapper.Map<Candidate>(request);
        var created = await candidateService.CreateAsync(candidateModel);
        return Ok(mapper.Map<CandidateResponse>(created));
    }

    [HttpPost("{id:int}/hire")]
    public async Task<IActionResult> HireCandidate(int id)
    {
        await candidateService.HireCandidateAsync(id);
        return Ok(new { message = "Candidate hired successfully, employee record created, and invite sent." });
    }
}