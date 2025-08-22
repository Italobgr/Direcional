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




        // validação de autenticação - trocar
        if (req.Username == "corretor" && req.Password == "12345")
        {
            var token = tokenSvc.GeraToken(req.Username, "Corretor");
            return Ok(new { token });
        }
        return Unauthorized();
    }
}
