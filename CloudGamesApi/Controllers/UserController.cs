using Core;
using Core.Dtos;
using Core.Entity;
using Core.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudGamesApi.Controllers;

[ApiController]
[Route("/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    
    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
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
            var user = new User()
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Password = userDto.Password,
                Permission = userDto.Permission.ToLower(),
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