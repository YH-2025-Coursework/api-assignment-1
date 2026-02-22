using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;
using Workshop.Api.Options;

namespace Workshop.Api.Controllers;

[ApiController]
[Route("api/auth")]
// Issues short-lived JWTs after validating the configured demo password.
public sealed class AuthController(IOptions<JwtSettings> jwtOptions) : ControllerBase
{
    [HttpPost("token")]
    [AllowAnonymous]
    public ActionResult<TokenResponse> CreateToken([FromBody] TokenRequest request)
    {
        var settings = jwtOptions.Value;
        // Fail fast if the demo password isn't configured so we don't issue tokens by accident.
        if (string.IsNullOrWhiteSpace(settings.DemoPassword))
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "JWT demo password is not configured.");
        }

        if (!string.Equals(request.Password, settings.DemoPassword, StringComparison.Ordinal))
        {
            return Unauthorized();
        }

        // Build the claims that go inside the JWT.
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, "workshop-admin"),  // Identifies the subject/user - hardcoded to "workshop-admin".
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),  // A unique token ID (guid) so each issued token gets a distinct identifier.
            new(ClaimTypes.Role, "Admin")  // Sets the user’s role to "Admin", so downstream [Authorize(Roles = "Admin")] checks could pass.
        };

        // SymmetricSecurityKey wraps the byte array for our Jwt:Key - that’s the secret used to sign tokens (HMAC).
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));

        // SigningCredentials pairs that key with a signing algorithm (HS256 in this case). It tells the JWT handler how to
        // generate the signature portion of the token. Without SigningCredentials, the JWT can’t be signed / validated.
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new TokenResponse(tokenString));
    }

    public sealed record TokenRequest(string Password);  // Describes the JSON payload our /api/auth/token endpoint expects ({ "password": "…" }). 
    public sealed record TokenResponse(string Token);  // The shape of the JSON we return ({ "token": "…" }).
}
