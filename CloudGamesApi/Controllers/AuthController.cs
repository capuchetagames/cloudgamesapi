using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CloudGamesApi.Service;
using Core;
using Core.Dtos;
using Core.Repository;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CloudGamesApi.Controllers;

/// <summary>
/// Gerencia a autenticação de usuários e a geração de tokens JWT.
/// </summary>
[ApiController]
[Route("/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;
    
    private IValidator<LoginDto>  _loginDtoValidator;

    private BaseLogger<AuthController> _logger;

    public AuthController(IConfiguration configuration, IUserRepository  userRepository,
        IValidator<LoginDto> loginDtoValidator, BaseLogger<AuthController> logger)
    {
        _configuration = configuration;
        _userRepository =  userRepository;
        _loginDtoValidator =  loginDtoValidator;
        _logger =  logger;
    }

    /// <summary>
    /// Autentica um usuário e retorna um token JWT.
    /// </summary>
    /// <remarks>
    /// Este endpoint valida as credenciais do usuário.<br/>
    /// Em caso de sucesso, gera um token JWT com validade de 30 minutos,
    /// contendo as claims 'sub' (username) e 'role' (permissão).
    /// </remarks>
    /// <param name="loginDto">Objeto contendo o 'Name' (username) e 'Password'.</param>
    /// <returns>Um objeto contendo o token JWT.</returns>
    [HttpPost]
    [AllowAnonymous]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var validationResult = await _loginDtoValidator.ValidateAsync(loginDto);
        
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary()); 
            }
        
            _logger.LogInformation($"Tentativa de login para o usuário: {loginDto.Name}");
        
            var user = _userRepository.GetUserByUsername(loginDto.Name);
        
            if(user == null || user.Password != loginDto.Password)
            {
                _logger.LogWarning($"Falha na autenticação: {loginDto.Name}. Usuário ou senha inválidos.");
                return Unauthorized(new { message = "Usuário ou senha inválidos." });
            }
        
            _logger.LogInformation($"Usuário {loginDto.Name} autenticado com sucesso.");
        
        
            var token = GenerateToken(user.Name, user.Permission);
            
            return Ok(new LoginResponseDto { Token = token });

        }
        catch (Exception e)
        {
            _logger.LogError($"{e.Message} / Erro inesperado durante o login.");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno."});
        }
    }
    
    /// <summary>
    /// Gera um token JWT para o usuário especificado.
    /// </summary>
    /// <param name="username">O nome de usuário (será usado como 'sub').</param>
    /// <param name="role">O nível de permissão (será usado como 'role').</param>
    /// <returns>Uma string contendo o token JWT.</returns>
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