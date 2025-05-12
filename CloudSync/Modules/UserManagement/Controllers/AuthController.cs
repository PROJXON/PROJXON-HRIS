using CloudSync.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.User;

namespace CloudSync.Modules.UserManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(DatabaseContext context) : ControllerBase
{
    private readonly DatabaseContext _context = context;

    [HttpPost("google-login")]
    public async Task<string> GoogleLogIn(LoginDTO loginData)
    {
        return "test";
    }
    
    // TODO logout
    // TODO update password
}