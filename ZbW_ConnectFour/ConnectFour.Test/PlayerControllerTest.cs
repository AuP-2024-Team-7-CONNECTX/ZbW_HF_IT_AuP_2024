using ConnectFour.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using ConnectFour.Api.User;
using ConnectFour.Repositories;
using Moq;

namespace ConnectFour.Tests
{
    [TestClass]
    public class PlayerControllerTests
    {
        private GameDbContext _context;
        private PlayerController _controller;

        [TestInitialize]
        public void Setup()
        {
            var dbContextOptions = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique name for each test run
                .Options;

            _context = new GameDbContext(dbContextOptions);

            // Initialize the database with necessary initial data
            TestConfigurationHelper.InitializeDbForTests(_context);

            var loggerGenericRepository = new Mock<ILogger<GenericRepository>>().Object;
            var loggerPlayerController = new Mock<ILogger<PlayerController>>().Object;


            var genericRepository = new GenericRepository(_context, loggerGenericRepository);
            var playerRepository = new PlayerRepository(genericRepository);
            var userRepository = new UserRepository(genericRepository);

            _controller = new PlayerController(playerRepository, userRepository, loggerPlayerController);
        }

       

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetAll_ReturnsAllPlayers()
        {
            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var actionResult = result.Result as OkObjectResult;
            var players = actionResult.Value as List<PlayerResponse>;
            Assert.AreEqual(2, players.Count);
        }

        [TestMethod]
        public async Task Get_ReturnsPlayerById_WhenPlayerExists()
        {
            // Arrange
            var testPlayerId = _context.Players.First().Id;

            // Act
            var result = await _controller.Get(testPlayerId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var actionResult = result.Result as OkObjectResult;
            var player = actionResult.Value as PlayerResponse;
            Assert.AreEqual(testPlayerId, player.Id);
        }

        [TestMethod]
        public async Task Post_ReturnsBadRequest_WhenUserNotFound()
        {
            // Arrange
            var newPlayerRequest = new PlayerRequest { Name = "New Player", UserId = "NonExistingUser", IsIngame = false };

            // Act
            var result = await _controller.Post(newPlayerRequest);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
            var actionResult = result.Result as ObjectResult;
            Assert.AreEqual(404, actionResult.StatusCode);
        }

        [TestMethod]
        public async Task Put_UpdatesPlayer_WhenPlayerExists()
        {
            // Arrange
            var existingPlayer = _context.Players.First();
            var updateRequest = new PlayerRequest { Name = "Updated Name", UserId = existingPlayer.UserId, IsIngame = true };

            // Act
            var result = await _controller.Put(existingPlayer.Id, updateRequest);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _context.Entry(existingPlayer).Reload(); // Reload the player from the DB to get updated values
            Assert.AreEqual("Updated Name", existingPlayer.Name);
            Assert.IsTrue(existingPlayer.IsIngame);
        }

        [TestMethod]
        public async Task Delete_RemovesPlayer_WhenPlayerExists()
        {
            // Arrange
            var existingPlayerId = _context.Players.First().Id;

            // Act
            var result = await _controller.Delete(existingPlayerId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            var deletedPlayer = _context.Players.Find(existingPlayerId);
            Assert.IsNull(deletedPlayer);
        }

    }
}
