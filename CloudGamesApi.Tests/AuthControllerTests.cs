using CloudGamesApi.Controllers;
using CloudGamesApi.Service;
using Core;
using Core.Dtos;
using Core.Entity;
using Core.Models;
using Core.Repository;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace CloudGamesApi.Tests;

public class AuthControllerTests
{
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IValidator<LoginDto>> _mockValidator;
    private readonly Mock<IPasswordHasher<User>> _mockPasswordHasher;
    
    private readonly Mock<IBaseLogger<AuthController>> _mockLogger; 

    
    private readonly AuthController _controller;
    
    public AuthControllerTests()
    {
        _mockConfig = new Mock<IConfiguration>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockValidator = new Mock<IValidator<LoginDto>>();
        _mockPasswordHasher = new Mock<IPasswordHasher<User>>();
        _mockLogger = new Mock<IBaseLogger<AuthController>>();

        const string jwtTestKey = "ESTA_EH_UMA_CHAVE_SECRETA_LONGA_PARA_TESTES_12345";
        
        var mockJwtKeySection = new Mock<IConfigurationSection>();
        mockJwtKeySection.Setup(s => s.Value).Returns(jwtTestKey);
        
        var mockJwtIssuerSection = new Mock<IConfigurationSection>();
        mockJwtIssuerSection.Setup(s => s.Value).Returns("MeuIssuerDeTeste");

        _mockConfig.Setup(c => c.GetSection("Jwt:Key")).Returns(mockJwtKeySection.Object);
        _mockConfig.Setup(c => c.GetSection("Jwt:Issuer")).Returns(mockJwtIssuerSection.Object);
        
        _controller = new AuthController(
            _mockConfig.Object,
            _mockUserRepo.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            _mockPasswordHasher.Object
        );
    }
    
    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("diogo123");
    
        var loginDto = new LoginDto { Name = "diogo", Password = "diogo123" };
    
        var userFromDb = new User 
        { 
            Id = 3, 
            Name = "diogo",
            Email = "diogocarlomagno@gmail.com",
            Password = hashedPassword,
            Permission = PermissionType.User
        };
    
        // 2. validador para retornar SUCESSO
        _mockValidator
            .Setup(v => v.ValidateAsync(loginDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // 3. Configura o repositório para ENCONTRAR o usuário
        _mockUserRepo
            .Setup(r => r.GetUserByUsername(loginDto.Name))
            .Returns(userFromDb);

        // 4. Configurar o hasher para dizer que a senha BATE
        _mockPasswordHasher
            .Setup(h => h.VerifyHashedPassword(userFromDb, userFromDb.Password, loginDto.Password))
            .Returns(PasswordVerificationResult.Success);

        // ----- Act 
        var result = await _controller.Login(loginDto);

        // ----- Assert 

        // 1. Verifica se o resultado é do tipo OkObjectResult (HTTP 200)
        var okResult = Assert.IsType<OkObjectResult>(result);
        
        // 2. Verifica se o valor retornado é do tipo LoginResponseDto
        var responseDto = Assert.IsType<LoginResponseDto>(okResult.Value);

        // 3. Verifica se o token foi gerado
        Assert.False(string.IsNullOrEmpty(responseDto.Token));
        
        
    }
    
    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsInvalid()
    {
        var loginDto = new LoginDto { Name = "diogo", Password = "senha_errada" };
        var userFromDb = new User 
        { 
            Id = 1, 
            Name = "diogo", 
            Email = "diogocarlomagno@gmail.com",
            Password = "AQAAAAIAAYagAAAAECev5vnuTLZM6LgNPpBPaf5Jl4r/yd3QSTyISMDcYLniPDkn8L/hDCjy1Sh2mxnCtw==", 
            Permission = PermissionType.Admin 
        };

        _mockValidator
            .Setup(v => v.ValidateAsync(loginDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        
        _mockUserRepo
            .Setup(r => r.GetUserByUsername(loginDto.Name))
            .Returns(userFromDb);

        
        _mockPasswordHasher
            .Setup(h => h.VerifyHashedPassword(userFromDb, userFromDb.Password, loginDto.Password))
            .Returns(PasswordVerificationResult.Failed);

        var result = await _controller.Login(loginDto);

        // 1. Verifica se o resultado é do tipo UnauthorizedObjectResult (HTTP 401)
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);

        var message = unauthorizedResult.Value.GetType().GetProperty("message").GetValue(unauthorizedResult.Value);
         Assert.Equal("Usuário ou senha inválidos.", message);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenValidationFails()
    {
        var loginDto = new LoginDto { Name = "diogo", Password = "" };
        
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Password", "O campo 'Password' é obrigatório.")
        };
        var validationResult = new ValidationResult(validationFailures);

        _mockValidator
            .Setup(v => v.ValidateAsync(loginDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // ----- Act 
        var result = await _controller.Login(loginDto);

        // ----- Assert 

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        
        Assert.NotNull(badRequestResult.Value);
        Assert.IsAssignableFrom<IDictionary<string, string[]>>(badRequestResult.Value);
    }
}