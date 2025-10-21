using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core;
using Core.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CloudGamesApi.Controllers;

[ApiController]
[Route("/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;

    public AuthController(IConfiguration configuration, IUserRepository  userRepository)
    {
        _configuration = configuration;
        _userRepository =  userRepository;
    }

    [HttpPost]
    public IActionResult Login([FromBody] string username, string password)
    {
        
        
        
        
        if (username == "admin" && password == "admin")
        {
            var token = GenerateToken(username, PermissionType.Admin);
            return Ok(new {token});
        }
        else if (username == "user" && password == "user")
        {
            var token = GenerateToken(username, PermissionType.User);
            return Ok(new {token});
        }
        else
        {
            return Unauthorized();
        }
    }

    private string GenerateToken(string username, PermissionType role)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(ClaimTypes.Role, role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer:_configuration["Jwt:Issuer"],
            claims:claims,
            expires:DateTime.Now.AddMinutes(30),
            signingCredentials:credential);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}