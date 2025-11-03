using CloudGamesApi.Controllers;
using Core.Dtos;
using Core.Entity;
using Core.Models;
using Core.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace CloudGamesApi.Tests;

public class GamesControllerTests
{
    private readonly Mock<IGameRepository> _mockRepo;
    private readonly Mock<ICacheService> _mockCache;
    private readonly GamesController _controller;
    private readonly List<Game> _testGames;

    public GamesControllerTests()
    {
        _mockRepo = new Mock<IGameRepository>();
        _mockCache = new Mock<ICacheService>();
        
        _controller = new GamesController(_mockRepo.Object, _mockCache.Object);
        
        _testGames = new List<Game>
        {
            new Game { Id = 1, Name = "Stardew Valley", Category = "Simulação", Price = 24.99m, Active = true },
            new Game { Id = 2, Name = "Baldur's Gate", Category = "RPG", Price = 199.99m, Active = true },
            new Game { Id = 3, Name = "Hollow Knight", Category = "Ação-Aventura", Price = 46.99m, Active = true },
            new Game { Id = 4, Name = "Cyberpunk 2077", Category = "Mundo Aberto", Price = 199.99m, Active = true },
            new Game { Id = 5, Name = "Civilization VI", Category = "Estratégia", Price = 129.99m, Active = true }
        };
    }
    
    [Fact]
    public void Get_ShouldReturnOk_WhenCacheIsHit()
    {
        _mockCache.Setup(c => c.Get("gameList")).Returns(_testGames);

        var result = _controller.Get();

        var okResult = Assert.IsType<OkObjectResult>(result);
        
        Assert.Equal(_testGames, okResult.Value);
        
        _mockRepo.Verify(r => r.GetAll(), Times.Never());
    }
    
    [Fact]
    public void Get_ShouldReturnOk_WhenCacheIsMissed()
    {
        _mockCache.Setup(c => c.Get("gameList")).Returns((List<Game>)null);
        
        _mockRepo.Setup(r => r.GetAll()).Returns(_testGames);

        var result = _controller.Get();

        var okResult = Assert.IsType<OkObjectResult>(result);
        
        Assert.Equal(_testGames, okResult.Value);
        
        _mockRepo.Verify(r => r.GetAll(), Times.Once());
        
        _mockCache.Verify(c => c.Set("gameList", _testGames), Times.Once());
    }
    
    [Fact]
    public void GetById_ShouldReturnNotFound_WhenGameDoesNotExist()
    {
        var gameId = 99;
        
        _mockCache.Setup(c => c.Get($"game-{gameId}")).Returns((Game)null);
        
        _mockRepo.Setup(r => r.GetById(gameId)).Returns((Game)null);

        var result = _controller.Get(gameId);

        Assert.IsType<NotFoundObjectResult>(result);
        
        _mockCache.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<object>()), Times.Never());
    }
    
    [Fact]
    public void GetById_ShouldReturnOk_WhenCacheIsMissedAndRepoFinds()
    {
        var game = _testGames[0];
        
        _mockCache.Setup(c => c.Get($"game-{game.Id}")).Returns((Game)null);
        
        _mockRepo.Setup(r => r.GetById(game.Id)).Returns(game);

        var result = _controller.Get(game.Id);

        var okResult = Assert.IsType<OkObjectResult>(result);
        
        Assert.Equal(game, okResult.Value);
        
        _mockRepo.Verify(r => r.GetById(game.Id), Times.Once());
        _mockCache.Verify(c => c.Set($"game-{game.Id}", game), Times.Once());
    }
    
    [Fact]
    public void Post_ShouldReturnCreatedAtAction_WhenSuccessful()
    {
        var input = new GameInput { Name = "New Game", Category = "Test", Price = 10 };
        
        _mockRepo.Setup(r => r.Add(It.IsAny<Game>()))
            .Callback<Game>(g => g.Id = 100);

        var result = _controller.Post(input);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        
        var gameValue = Assert.IsType<Game>(createdResult.Value);
        
        Assert.Equal(100, gameValue.Id);
        
        Assert.Equal(input.Name, gameValue.Name);
        
        _mockRepo.Verify(r => r.Add(It.IsAny<Game>()), Times.Once());
    }
    
    [Fact]
    public void Put_ShouldReturnNoContent_WhenSuccessful()
    {
        var originalGame = _testGames[0];
        var input = new UpdateGameInput 
        { 
            Id = originalGame.Id, 
            Name = "UPDATED NAME", // Novo nome
            Category = originalGame.Category,
            Price = originalGame.Price,
            Active = originalGame.Active
        };
        
        _mockRepo.Setup(r => r.GetById(input.Id)).Returns(originalGame);

        var result = _controller.Put(input);

        Assert.IsType<NoContentResult>(result);
        
        _mockRepo.Verify(r => r.Update(It.IsAny<Game>()), Times.Once());
        
        Assert.Equal("UPDATED NAME", originalGame.Name);
    }
    
    [Fact]
    public void Put_ShouldReturnNotFound_WhenGameDoesNotExist()
    {
        
        var input = new UpdateGameInput { Id = 99, Name = "Fantasma" };
        
        _mockRepo.Setup(r => r.GetById(input.Id)).Returns((Game)null);

        var result = _controller.Put(input);

        Assert.IsType<NotFoundObjectResult>(result);
        
        _mockRepo.Verify(r => r.Update(It.IsAny<Game>()), Times.Never());
    }
    
    [Fact]
    public void Delete_ShouldReturnNoContent_WhenSuccessful()
    {
        var gameToDelete = _testGames[0];
        
        _mockRepo.Setup(r => r.GetById(gameToDelete.Id)).Returns(gameToDelete);

        var result = _controller.Delete(gameToDelete.Id);

        Assert.IsType<NoContentResult>(result);
        
        _mockRepo.Verify(r => r.Delete(gameToDelete.Id), Times.Once());
    }
    
    [Fact]
    public void Delete_ShouldReturnNotFound_WhenGameDoesNotExist()
    {
        var gameId = 99;
        
        _mockRepo.Setup(r => r.GetById(gameId)).Returns((Game)null);

        var result = _controller.Delete(gameId);

        Assert.IsType<NotFoundObjectResult>(result);
        
        _mockRepo.Verify(r => r.Delete(gameId), Times.Never());
    }
    
    [Fact]
    public void Get_ShouldReturn500_WhenRepositoryThrowsException()
    {
        _mockRepo.Setup(r => r.GetAll()).Throws(new Exception("Erro de banco de dados"));

        var result = _controller.Get();

        var objectResult = Assert.IsType<ObjectResult>(result);
        
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
    }
}