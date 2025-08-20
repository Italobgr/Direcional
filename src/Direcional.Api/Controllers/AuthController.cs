using Direcional.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Direcional.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ITokenService tokenSvc) : ControllerBase
{
    public record LoginRequest(string Username, string Password);

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        // Validação fake só para POC. Em prod, valide em banco.
        if (req.Username == "corretor" && req.Password == "123")
        {
            var token = tokenSvc.GenerateToken(req.Username, "Corretor");
            return Ok(new { token });
        }
        return Unauthorized();
    }
}
