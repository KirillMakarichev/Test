using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestApp1.Contracts.Auth;
using TestApp1.Services.Interfaces;

namespace TestApp1.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest();
        }
        
        var response = await authService.LoginAsync(request);
        if (response is null)
        {
            return Unauthorized();
        }

        return Ok(response);
    }
}
