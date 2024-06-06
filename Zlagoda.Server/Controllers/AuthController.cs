using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Zlagoda.Server.Models;

namespace Zlagoda.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<TestController> _logger;

    public AuthController(IOptions<JwtOptions> jwtOptions, ILogger<TestController> logger)
    {
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    private async Task<string?> CreateToken(string user, string role)
    {
        try
        {
            if (_jwtOptions == null)
                _logger.LogError("JwtOptions is null");
            _logger.LogInformation($"Key: {_jwtOptions.Key}");
            var claims = new List<Claim>
            {
                new Claim("uid", user),
                new Claim("roles", role)
            };
            var now = DateTime.UtcNow;
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.Key));
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                notBefore: now,
                claims: claims,
                expires: now.Add(TimeSpan.FromMinutes(_jwtOptions.ExpiresMinutes)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateToken error");
            return null;
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<bool?>> Login(LoginPassModel loginPass)
    {
        if (loginPass == null || string.IsNullOrEmpty(loginPass.Login) || string.IsNullOrEmpty(loginPass.Password))
            return Unauthorized();
        var user = loginPass.Login == "John" && loginPass.Password == "123" ||
            loginPass.Login == "Kate" && loginPass.Password == "321" ? loginPass.Login : null;
        if (user != null)
        {
            var tokenStr = await CreateToken(user, user == "John" ? "Manager" : "Cashier");
            Response.Headers.Append(HttpRequestHeader.Authorization.ToString(), tokenStr);
            return Ok(new UserLoginModel
            {
                Login = user,
                FirstName = user,
                Role = user == "John" ? "Manager" : "Cashier"
            });
        }
        return Unauthorized();
    }

    [HttpGet("refresh")]
    public async Task<ActionResult> Refresh()
    {
        var claims = User?.Claims;
        var header = Request.Headers.FirstOrDefault(h => h.Key == HttpRequestHeader.Authorization.ToString());
        if (header.Value.Count == 0 || !header.Value[0]!.StartsWith("Bearer ", true, CultureInfo.InvariantCulture))
        {
            return Unauthorized();
        }
        var token = header.Value[0]!.Substring(header.Value[0]!.IndexOf(' ') + 1);
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtOptions.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.Key)),
            ValidateLifetime = false,
            //ValidateLifetime = true,
            //LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken,
            //    TokenValidationParameters validationParameters) =>
            //{
            //    var clonedParameters = validationParameters.Clone();
            //    clonedParameters.LifetimeValidator = null;
            //    var exp = expires?.Add(TimeSpan.FromDays(_jwtOptions.RefreshExpiresDays));
            //    Validators.ValidateLifetime(notBefore, exp, securityToken, clonedParameters);
            //    return true;
            //}
        };
        string? user = null;
        string? role = null;
        try
        {
            var principal = new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out var securityToken);
            user = principal.Claims.SingleOrDefault(c => c.Type == "uid")?.Value;
            role = principal.Claims.SingleOrDefault(c => c.Type.EndsWith("role"))?.Value;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Refresh error");
        }
        if (user == null)
        {
            return Unauthorized();
        }
        var tokenStr = await CreateToken(user, role);
        Response.Headers.Append(HttpRequestHeader.Authorization.ToString(), tokenStr);

        return Ok();
    }
}
