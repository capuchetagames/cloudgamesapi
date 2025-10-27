using Core;
using Core.Dtos;
using Core.Entity;
using Core.Models;
using Core.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudGamesApi.Controllers;

[ApiController]
[Route("/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameRepository _gameRepository;
    private readonly ICacheService _cacheService;
    public GamesController(IGameRepository gameRepository, ICacheService cacheService)
    {
        _gameRepository = gameRepository;
        _cacheService = cacheService;
    }

    [HttpGet]
    [Authorize]
    public IActionResult Get()
    {
        try
        {
            var gameListKey = "gameList";
            
            var cachedGameList = _cacheService.Get(gameListKey);

            if (cachedGameList != null)
            {
                return Ok(cachedGameList);
            }
            
            var gameList = _gameRepository.GetAll();
            
            _cacheService.Set(gameListKey, gameList);
            
            return Ok(gameList);
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
            var gameKey = $"game-{id}";
            
            var cachedGame = _cacheService.Get(gameKey);
            
            if (cachedGame != null)
            {
                return Ok(cachedGame);
            }

            var game = _gameRepository.GetById(id);
            
            _cacheService.Set(gameKey, game);
            
            return Ok(game);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpPost]
    [Authorize(Policy = nameof(PermissionType.Admin))]
    public IActionResult Post([FromBody] GameInput gameInput)
    {
        try
        {
            var game = new Game()
            {
                Name = gameInput.Name,
                Category = gameInput.Category,
                Active = gameInput.Active,
                Price = gameInput.Price,
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
    public IActionResult Put([FromBody] UpdateGameInput gameInput)
    {
        try
        {
            var game = _gameRepository.GetById(gameInput.Id);
            game.Name = gameInput.Name;
            game.Category = gameInput.Category;
            game.Active = gameInput.Active;
            game.Price = gameInput.Price;
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