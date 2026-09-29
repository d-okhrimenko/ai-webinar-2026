using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ReviewsAssistant.WebAPI.Contracts.Auth;

namespace ReviewsAssistant.WebAPI.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IConfiguration configuration) : ControllerBase
{
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login(LoginInput input)
    {
        var admin = configuration.GetSection("Admin");

        if (!string.Equals(input.Email, admin["Email"], StringComparison.OrdinalIgnoreCase)
            || input.Password != admin["Password"])
        {
            return Unauthorized(new { message = "Невірний email або пароль." });
        }

        var jwt = configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["SigningKey"]!));

        var token = new JwtSecurityToken(
            jwt["Issuer"],
            jwt["Audience"],
            [new Claim(ClaimTypes.Name, input.Email)],
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return Ok(new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token)));
    }
}
