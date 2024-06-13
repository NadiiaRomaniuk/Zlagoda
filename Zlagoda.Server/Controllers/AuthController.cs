using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Zlagoda.Server.Database;
using Zlagoda.Server.Models;
using System.Security.Cryptography;

namespace Zlagoda.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<AuthController> _logger;
    private readonly Db _db;

    public AuthController(IOptions<JwtOptions> jwtOptions, ILogger<AuthController> logger, Db db)
    {
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
        _db = db;
    }

    public static string GetHash(string password, string salt)
    {
        using (SHA1 sha1 = SHA1.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password + salt);
            byte[] hashBytes = sha1.ComputeHash(bytes);
            StringBuilder hexString = new StringBuilder();
            foreach (byte b in hashBytes)
                hexString.Append(b.ToString("x2"));
            return hexString.ToString();
        }
    }

    private async Task<string?> CreateToken(UserLoginModel user)
    {
        try
        {
            var claims = new List<Claim>
            {
                new Claim("uid", user.Login),
                new Claim("name", user.Name),
                new Claim("roles", user.Roles[0])
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

        var user = await _db.GetEmployee(loginPass.Login);
        if (user != null && user.Password == GetHash(loginPass.Password, _jwtOptions.Key))
        {
            var model = new UserLoginModel
            {
                Login = loginPass.Login,
                Name = $"{user.Surname} {user.Name}",
                Roles = [ user.Role ]
            };
            var tokenStr = await CreateToken(model);
            Response.Headers.Append(HttpRequestHeader.Authorization.ToString(), tokenStr);
            return Ok(model);
        }
        else if (await _db.GetEmployeesCount() == 0 && loginPass.Login == "admin" && loginPass.Password == "admin")
        {
            var model = new UserLoginModel
            {
                Login = "admin",
                Name = "Admin",
                Roles = [ "manager" ]
            };
            var tokenStr = await CreateToken(model);
            Response.Headers.Append(HttpRequestHeader.Authorization.ToString(), tokenStr);
            return Ok(model);
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
            ValidateLifetime = true,
            LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken,
                TokenValidationParameters validationParameters) =>
            {
                var clonedParameters = validationParameters.Clone();
                clonedParameters.LifetimeValidator = null;
                var exp = expires?.Add(TimeSpan.FromDays(_jwtOptions.RefreshExpiresDays));
                Validators.ValidateLifetime(notBefore, exp, securityToken, clonedParameters);
                return true;
            }
        };
        string? login = null;
        string? name = null;
        string? role = null;
        try
        {
            var principal = new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out var securityToken);
            name = principal.Claims.SingleOrDefault(c => c.Type == "name")?.Value;
            login = principal.Claims.SingleOrDefault(c => c.Type == "uid")?.Value;
            role = principal.Claims.SingleOrDefault(c => c.Type.EndsWith("role"))?.Value;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Refresh error");
        }
        if (login == null || role == null)
        {
            return Unauthorized();
        }
        var model = new UserLoginModel
        {
            Login = login,
            Name = name,
            Roles = [ role ]
        };
        var tokenStr = await CreateToken(model);
        Response.Headers.Append(HttpRequestHeader.Authorization.ToString(), tokenStr);

        return Ok();
    }
}
