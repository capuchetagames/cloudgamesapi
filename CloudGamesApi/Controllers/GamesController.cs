using Core;
using Core.Dtos;
using Core.Entity;
using Core.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudGamesApi.Controllers;

[ApiController]
[Route("/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameRepository _gameRepository;
    public GamesController(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    [HttpGet]
    [Authorize]
    public IActionResult Get()
    {
        try
        {
            return Ok(_gameRepository.GetAll());
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public IActionResult Get([FromRoute] int id)
    {
        try
        {
            return Ok(_gameRepository.GetById(id));
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpPost]
    [Authorize(Policy = nameof(PermissionType.Admin))]
    public IActionResult Post([FromBody] GameDto gameDto)
    {
        try
        {
            var game = new Game()
            {
                Name = gameDto.Name,
                Category = gameDto.Category,
                Active = gameDto.Active,
                Price = gameDto.Price,
            };
            _gameRepository.Add(game);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpPut]
    [Authorize(Policy = nameof(PermissionType.Admin))]
    public IActionResult Put([FromBody] UpdateGameDto gameDto)
    {
        try
        {
            var game = _gameRepository.GetById(gameDto.Id);
            game.Name = gameDto.Name;
            game.Category = gameDto.Category;
            game.Active = gameDto.Active;
            game.Price = gameDto.Price;
            _gameRepository.Update(game);
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
            _gameRepository.Delete(id);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
    
    
}