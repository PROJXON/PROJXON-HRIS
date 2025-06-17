using Microsoft.AspNetCore.Mvc;
using CloudSync.Modules.UserManagement.Services.Interfaces;
using Shared.Requests.UserManagement;

namespace CloudSync.Modules.UserManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        var response = await authService.LoginAsync(request);
        return Ok(response);
    }
}