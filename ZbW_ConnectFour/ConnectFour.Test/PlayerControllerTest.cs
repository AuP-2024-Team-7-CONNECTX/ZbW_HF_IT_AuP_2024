using ConnectFour.Controllers;
using ConnectFour.Models;
using ConnectFour.Repositories.Implementations;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectFour.Tests
{
    [TestClass]
    public class PlayerControllerTests
    {
        private GameDbContext _context;
        private PlayerController _controller;
        private UserRepository _userRepository;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new GameDbContext(options);
            TestConfigurationHelper.InitializeDbForTests(_context);

            var loggerMockPlayer = new Mock<ILogger<PlayerController>>();
            var loggerMockUser = new Mock<ILogger<UserController>>();
            var userRepository = new UserRepository(new GenericRepository(_context, new Mock<ILogger<GenericRepository>>().Object));

            var playerRepository = new PlayerRepository(new GenericRepository(_context, new Mock<ILogger<GenericRepository>>().Object), userRepository, new Mock<ILogger<PlayerRepository>>().Object);

            _userRepository = userRepository;
            _controller = new PlayerController(playerRepository, userRepository, loggerMockPlayer.Object);
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
            var result = await _controller.GetAll();
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var actionResult = result.Result as OkObjectResult;
            var players = actionResult.Value as IEnumerable<PlayerResponse>;
            Assert.IsTrue(players.Any());
            Assert.AreEqual(2, players.Count());
        }

        [TestMethod]
        public async Task GetById_ReturnsOnePlayer()
        {
            var existingUsers = await _userRepository.GetAllAsync();
            var existingUser = existingUsers.FirstOrDefault();

            Assert.IsNotNull(existingUser);

            var newPlayerRequest = new PlayerRequest { Name = "Test Player", UserId = existingUser.Id };
            var postResult = await _controller.Post(newPlayerRequest);

            var actionPostResult = postResult.Result as CreatedAtActionResult;
            
            var newPlayer = actionPostResult.Value as PlayerResponse;
            var id = newPlayer.Id;

            var result = await _controller.Get(id);
            
            var actionResult = result.Result as OkObjectResult;
            var player = actionResult.Value as PlayerResponse;
            Assert.IsNotNull(player);
            Assert.AreEqual(id, player.Id);
        }

        [TestMethod]
        public async Task Get_ReturnsPlayerById_WhenPlayerExists()
        {
            var testPlayerId = _context.Players.First().Id.ToString();

            var result = await _controller.Get(testPlayerId);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var actionResult = result.Result as OkObjectResult;
            var player = actionResult.Value as PlayerResponse;
            Assert.AreEqual(testPlayerId, player.Id);
        }

        [TestMethod]
        public async Task Post_CreatesNewPlayer()
        {
            var existingUsers = await _userRepository.GetAllAsync();
            var existingUser = existingUsers.FirstOrDefault();

            Assert.IsNotNull(existingUser);

            var newPlayerRequest = new PlayerRequest { Name = "Test Player", UserId = existingUser.Id };

            var postResult = await _controller.Post(newPlayerRequest);
            Assert.IsInstanceOfType(postResult.Result, typeof(CreatedAtActionResult));

            var actionPostResult = postResult.Result as CreatedAtActionResult;
            var newPlayer = actionPostResult.Value as PlayerResponse;
            var id = newPlayer.Id;

            var newPlayer2 = _context.Players.Find(id);

            Assert.IsNotNull(newPlayer);
        }

        [TestMethod]
        public async Task Put_UpdatesPlayer_WhenPlayerExists()
        {
            var existingPlayer = _context.Players.First();
            var updatePlayerRequest = new PlayerRequest { Name = "Updated Name" };

            var result = await _controller.Put(existingPlayer.Id, updatePlayerRequest);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _context.Entry(existingPlayer).Reload();
            Assert.AreEqual("Updated Name", existingPlayer.Name);
        }

        [TestMethod]
        public async Task Delete_MarksPlayerAsDeleted_WhenPlayerExists()
        {
            var existingUsers = await _userRepository.GetAllAsync();
            var existingUser = existingUsers.FirstOrDefault();

            Assert.IsNotNull(existingUser);


            var newPlayerRequest = new PlayerRequest { Name = "Test Player", UserId = existingUser.Id };

            var postResult = await _controller.Post(newPlayerRequest);

            var actionPostResult = postResult.Result as CreatedAtActionResult;
            var newPlayer = actionPostResult.Value as PlayerResponse;
            var id = newPlayer.Id;

            _context.SaveChanges();

            var player = _context.Players.Find(id);
            Assert.IsNotNull(player);
            Assert.IsNull(player.DeletedOn);

            var resultDelete = await _controller.Delete(id);
            _context.SaveChanges();

            Assert.IsInstanceOfType(resultDelete, typeof(NoContentResult));
            Assert.IsNotNull(player.DeletedOn);
        }
    }
}
