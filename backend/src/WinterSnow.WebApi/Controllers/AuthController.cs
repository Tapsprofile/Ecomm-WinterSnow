using Microsoft.AspNetCore.Mvc;
using WinterSnow.Services.Iam;

namespace WinterSnow.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResult>> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        var result = await _auth.PasswordLoginAsync(req.Email, req.Password, ct);
        if (result is null)
            return Unauthorized(new { message = "Invalid credentials" });
        return Ok(result);
    }
}

