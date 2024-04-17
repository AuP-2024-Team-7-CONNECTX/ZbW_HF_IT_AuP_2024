using ConnectFour.Controllers;
using ConnectFour.Models;
using ConnectFour.Repositories;
using ConnectFour.Repositories.Implementations;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Drawing;
using static ConnectFour.Enums.Enum;

namespace ConnectFour.Tests
{
    [TestClass]
    public class GameControllerTest
    {
        private GameDbContext _context;
        private GameController _controller;
        private UserRepository _userRepository;
        private PlayerRepository _playerRepository;
        private RobotRepository _robotRepository;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new GameDbContext(options);

            var user = new User { Id = Guid.NewGuid().ToString(), Name = "TestUser", Authenticated = true,Email="test" };
            var user2 = new User { Id = Guid.NewGuid().ToString(), Name = "TestUser", Authenticated = true, Email = "test" };

            var testGame = new Game
            {
                Id = Guid.NewGuid().ToString(),
                           
                Players = new List<Player>
                {
                    new Player { Id = Guid.NewGuid().ToString(), Name = "Player 1" ,User = user},
                    new Player { Id = Guid.NewGuid().ToString(), Name = "Player 2" ,User = user2}
                },
                Robots = new List<Robot>
                {
                    new Robot { Id = Guid.NewGuid().ToString(), Name = "Robot 1" }
                },
                State = GameState.Active,
                CurrentMove = null, // Assuming CurrentMove is a defined class
                WinnerPlayer = null,
                TotalPointsPlayerOne = 100,
                TotalPointsPlayerTwo = 95
            };

            _context.Games.Add(testGame);
            _context.SaveChanges();

            var loggerMock = new Mock<ILogger<GameController>>();
            var genericRepository = new GenericRepository(_context, new Mock<ILogger<GenericRepository>>().Object);
            var moveRepository = new MoveRepository(genericRepository);
            var gameRepository = new GameRepository(genericRepository, moveRepository);
            var userRepository = new UserRepository(genericRepository);
            var playerRepository = new PlayerRepository(genericRepository,userRepository, new Mock<ILogger<PlayerRepository>>().Object);
            var robotRepository = new RobotRepository(genericRepository, playerRepository, new Mock<ILogger<RobotRepository>>().Object);

            _userRepository = userRepository;
            _playerRepository = playerRepository;
            _controller = new GameController(gameRepository,playerRepository,robotRepository, loggerMock.Object);
            _robotRepository = robotRepository;
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetAllGames_ReturnsAllGames()
        {
            var result = await _controller.GetAll();

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var actionResult = result.Result as OkObjectResult;
            var games = actionResult.Value as IEnumerable<GameResponse>;
            Assert.IsNotNull(games);
            Assert.IsTrue(games.Any());
        }

        [TestMethod]
        public async Task GetGameById_ReturnsGame_WhenGameExists()
        {
            var gameId = _context.Games.First().Id; // Assuming there's an Id property
            var result = await _controller.Get(gameId);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var actionResult = result.Result as OkObjectResult;
            var gameResponse = actionResult.Value as GameResponse;
            Assert.IsNotNull(gameResponse);
            Assert.AreEqual(gameId, gameResponse.Id);
        }

        [TestMethod]
        public async Task CreateGame_CreatesNewGame()
        {
            var gameId = "";

            var players = await _playerRepository.GetAllAsync();

            var count = 0;
            var name = "";
            foreach (var player in players)
            {
                name += "Robot " + count;
                var newRobotRequest = new Robot
                {
                    CurrentPlayerId = player.Id,
                    Name = name,
                    Color = ConnectFourColor.Red,
                    IsConnected = false,
                    IsIngame = false
                };
                count++;

                await _robotRepository.CreateOrUpdateAsync(newRobotRequest);
            }
            // Arrange
           

            var newGame = new Game
            {
                Id = Guid.NewGuid().ToString(),

                Players = new List<Player>
                {
                    //new Player { Id = Guid.NewGuid().ToString(), Name = "Player 1" ,User = user},
                    //new Player { Id = Guid.NewGuid().ToString(), Name = "Player 2" ,User = user2}
                },
                Robots = new List<Robot>
                {
                    new Robot { Id = Guid.NewGuid().ToString(), Name = "Robot 1" }
                },
                State = GameState.Active,
                CurrentMove = null, // Assuming CurrentMove is a defined class
                WinnerPlayer = null,
                TotalPointsPlayerOne = 100,
                TotalPointsPlayerTwo = 95
            };

            var gameRequest = new GameRequest() { State = "Abandoned" };

            gameId = newGame.Id;
            var result = await _controller.Post(gameRequest);

            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var actionResult = result.Result as CreatedAtActionResult;
            var createdGame = actionResult.Value as Game;
            Assert.IsNotNull(createdGame);
            Assert.IsTrue(_context.Games.Any(g => g.Id == createdGame.Id));
        }

        [TestMethod]
        public async Task UpdateGame_UpdatesExistingGame()
        {
            var existingGame = _context.Games.First();
            existingGame.State = GameState.Completed; // Example of an update

            var gameRequest = new GameRequest() { State = "Abandoned" };

            var result = await _controller.Put(existingGame.Id, gameRequest);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            var dbGame = await _context.Games.FindAsync(existingGame.Id);
            Assert.AreEqual(GameState.Completed, dbGame.State);
        }

        [TestMethod]
        public async Task DeleteGame_DeletesExistingGame()
        {
            var gameToDelete = _context.Games.First(g => g.DeletedOn == null);

            var result = await _controller.Delete(gameToDelete.Id);
            _context.SaveChanges();
            Assert.IsTrue(gameToDelete.DeletedOn.HasValue);
           
        }
    }
}
