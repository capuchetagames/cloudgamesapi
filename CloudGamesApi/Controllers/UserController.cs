using Core;
using Core.Dtos;
using Core.Entity;
using Core.Repository;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudGamesApi.Controllers;

[ApiController]
[Route("/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    
    private IValidator<BaseUserDto> _validator;
    
    public UserController(IUserRepository userRepository, IValidator<BaseUserDto>  validator)
    {
        _userRepository = userRepository;
        _validator = validator;
    }
    
    [HttpGet] 
    [Authorize(Policy = nameof(PermissionType.Admin))]
    public IActionResult Get()
    {
        try
        {
            return Ok(_userRepository.GetAll());
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
    
    [HttpGet("{id:int}")]
    [Authorize(Policy = nameof(PermissionType.Admin))]
    public IActionResult Get([FromRoute] int id)
    {
        try
        {
            return Ok(_userRepository.GetById(id));
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpGet("{id:int}/games")]
    [Authorize]
    public IActionResult GetGames([FromRoute] int id)
    {
        try
        {
            return Ok(_userRepository.GetUserGames(id));
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpPost]
    [Authorize(Policy = nameof(PermissionType.Admin))]
    public IActionResult Post([FromBody] UserDto userDto)
    {
        try
        {
            var validationResult =  _validator.Validate(userDto);
        
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary()); 
            }
            
            var user = new User()
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Password = userDto.Password,
                Permission = userDto.Permission,
            };
            _userRepository.Add(user);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
    
    [HttpPost("/new-user")]
    public IActionResult PostNewUser([FromBody] BaseUserDto userDto)
    {
        try
        {
            var validationResult =  _validator.Validate(userDto);
        
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary()); 
            }
            
            var user = new User()
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Password = userDto.Password,
                Permission = PermissionType.User,
            };
            _userRepository.Add(user);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpPut]
    [Authorize(Policy = nameof(PermissionType.Admin))]
    public IActionResult Put([FromBody] UpdateUserDto userDto)
    {
        try
        {
            var validationResult =  _validator.Validate(userDto);
        
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary()); 
            }
            
            var user = _userRepository.GetById(userDto.Id);
            user.Name = userDto.Name;
            user.Email = userDto.Email;
            user.Password = userDto.Password;
            user.Permission = userDto.Permission;
            
            _userRepository.Update(user);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = nameof(PermissionType.Admin))]
    public IActionResult Delete([FromRoute] int id)
    {
        try
        {
            _userRepository.Delete(id);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
    
}