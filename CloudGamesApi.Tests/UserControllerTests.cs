using CloudGamesApi.Controllers;
using Core;
using Core.Dtos;
using Core.Entity;
using Core.Repository;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace CloudGamesApi.Tests;

public class UserControllerTests
{
    private readonly Mock<IUserRepository> _mockRepo;
    private readonly Mock<IValidator<BaseUserDto>> _mockValidator;
    private readonly Mock<IPasswordHasher<User>> _mockHasher;
    
    private readonly UserController _controller;

    private readonly List<User> _testUsers;
    
    public UserControllerTests()
    {
        _mockRepo = new Mock<IUserRepository>();
        _mockValidator = new Mock<IValidator<BaseUserDto>>();
        _mockHasher = new Mock<IPasswordHasher<User>>();

        _testUsers = new List<User>
        {
            new User { Id = 1, Name = "AdminUser", Email = "admin@test.com", Password = "hash1", Permission = PermissionType.Admin },
            new User { Id = 2, Name = "NormalUser", Email = "user@test.com", Password = "hash2", Permission = PermissionType.User }
        };

        
        _controller = new UserController(
            _mockRepo.Object,
            _mockValidator.Object,
            _mockHasher.Object
        );

        _mockValidator
            .Setup(v => v.Validate(It.IsAny<BaseUserDto>()))
            .Returns(new ValidationResult());
    }
    
    [Fact]
    public void Get_ShouldReturnOk_WithListOfUserDtos()
    {
        _mockRepo.Setup(r => r.GetAll()).Returns(_testUsers);

        var result = _controller.Get();

        var okResult = Assert.IsType<OkObjectResult>(result);
            
        var responseDtos = Assert.IsAssignableFrom<IEnumerable<UserResponseDto>>(okResult.Value);
            
        Assert.Equal(2, responseDtos.Count());
        Assert.DoesNotContain(responseDtos, dto => dto.GetType().GetProperty("Password") != null);
    }
    
    [Fact]
    public void Get_ShouldReturn500_WhenRepositoryThrowsException()
    {
        _mockRepo.Setup(r => r.GetAll()).Throws(new Exception("DB falhou"));

        var result = _controller.Get();

        var objectResult = Assert.IsType<ObjectResult>(result);
     
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
    }
    
    [Fact]
    public void GetById_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        _mockRepo.Setup(r => r.GetById(99)).Returns((User)null);

        var result = _controller.Get(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public void Post_ShouldReturnCreated_WhenValidationSucceeds()
    {
        // ----- Arrange -----
        var userInput = new UserInput 
        { 
            Name = "NewUser", 
            Email = "new@test.com", 
            Password = "plain_password", 
            ConfirmPassword = "plain_password", 
            Permission = PermissionType.Admin
        };
        var hashedPass = "hashed_password_123";

        _mockHasher.Setup(h => h.HashPassword(It.IsAny<User>(), userInput.Password)).Returns(hashedPass);

        _mockRepo.Setup(r => r.Add(It.IsAny<User>()))
            .Callback<User>(u => u.Id = 5);

        var result = _controller.Post(userInput);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        
        var responseDto = Assert.IsType<UserResponseDto>(createdResult.Value);
        
        Assert.Equal(5, responseDto.Id);
        Assert.Equal(userInput.Name, responseDto.Name);
        
        _mockHasher.Verify(h => h.HashPassword(It.IsAny<User>(), userInput.Password), Times.Once);
    }
    
    [Fact]
    public void Post_ShouldReturnBadRequest_WhenValidationFails()
    {
        // ----- Arrange -----
        var userInput = new UserInput
        {
            Name = "Bad",
            Email = "",
            Password = "",
            ConfirmPassword = "",
            Permission = PermissionType.User
        };
        
        var failures = new List<ValidationFailure> { new ValidationFailure("Email", "Email é obrigatório") };
        
        _mockValidator
            .Setup(v => v.Validate(It.IsAny<BaseUserDto>()))
            .Returns(new ValidationResult(failures));

        var result = _controller.Post(userInput);

        Assert.IsType<BadRequestObjectResult>(result);
        
        _mockRepo.Verify(r => r.Add(It.IsAny<User>()), Times.Never());
        _mockHasher.Verify(h => h.HashPassword(It.IsAny<User>(), It.IsAny<string>()), Times.Never());
    }
    
    [Fact]
    public void PostNewUser_ShouldReturnCreated_AndSetDefaultUserRole()
    {
        // ----- Arrange -----
        var userDto = new BaseUserDto
        {
            Name = "PublicUser",
            Email = "public@test.com",
            Password = "plain_password",
            ConfirmPassword = "plain_password"
        };
        
        User capturedUser = null;
        
        _mockRepo.Setup(r => r.Add(It.IsAny<User>()))
            .Callback<User>(u => 
            {
                u.Id = 6;
                capturedUser = u;
            });
            
        
        var result = _controller.PostNewUser(userDto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        
        var responseDto = Assert.IsType<UserResponseDto>(createdResult.Value);
        Assert.Equal(6, responseDto.Id);

        Assert.NotNull(capturedUser);
        Assert.Equal(PermissionType.User, capturedUser.Permission);
        Assert.Equal(userDto.Name, capturedUser.Name);
    }
    
    [Fact]
    public void Put_ShouldReturnNoContent_WhenSuccessful()
    {
        var existingUser = _testUsers[1]; 
        var userInput = new UpdateUserInput
        {
            Id = 2,
            Name = "Nome Atualizado", 
            Password = "nova_senha",
            ConfirmPassword = "nova_senha",
            Email = existingUser.Email,
            Permission = existingUser.Permission
        };

        
        _mockRepo.Setup(r => r.GetById(userInput.Id)).Returns(existingUser);
        
        _mockHasher.Setup(h => h.HashPassword(existingUser, userInput.Password)).Returns("novo_hash");

        var result = _controller.Put(userInput);

        Assert.IsType<NoContentResult>(result);
        
        _mockRepo.Verify(r => r.Update(existingUser), Times.Once());
        
        _mockHasher.Verify(h => h.HashPassword(existingUser, userInput.Password), Times.Once());
        
        Assert.Equal("Nome Atualizado", existingUser.Name);
        Assert.Equal("novo_hash", existingUser.Password);
    }

    [Fact]
    public void Delete_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        _mockRepo.Setup(r => r.GetById(99)).Returns((User)null);

        var result = _controller.Delete(99);

        Assert.IsType<NotFoundObjectResult>(result);
        _mockRepo.Verify(r => r.Delete(99), Times.Never());
    }
    
    [Fact]
    public void Delete_ShouldReturnNoContent_WhenSuccessful()
    {
        var userToDelete = _testUsers[0];
        
        _mockRepo.Setup(r => r.GetById(userToDelete.Id)).Returns(userToDelete);

        var result = _controller.Delete(userToDelete.Id);

        Assert.IsType<NoContentResult>(result);
        
        _mockRepo.Verify(r => r.Delete(userToDelete.Id), Times.Once());
    }
}