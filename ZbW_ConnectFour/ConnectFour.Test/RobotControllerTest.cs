using ConnectFour.Api.User;
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
using static ConnectFour.Enums.Enum;

namespace ConnectFour.Tests
{
    [TestClass]
    public class RobotControllerTest
    {
        private GameDbContext _context;
        private RobotController _controller;
        private PlayerRepository _playerRepository;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new GameDbContext(options);
            TestConfigurationHelper.InitializeDbForTests(_context); // Stellen Sie sicher, dass Sie eine Methode haben, die Ihre Testdaten initialisiert

            var loggerMock = new Mock<ILogger<RobotController>>();
            var userRepository = new UserRepository(new GenericRepository(_context, new Mock<ILogger<GenericRepository>>().Object));
            
            _playerRepository = new PlayerRepository(new GenericRepository(_context, new Mock<ILogger<GenericRepository>>().Object), userRepository, new Mock<ILogger<PlayerRepository>>().Object);
            var robotRepository = new RobotRepository(new GenericRepository(_context, new Mock<ILogger<GenericRepository>>().Object), _playerRepository, new Mock<ILogger<RobotRepository>>().Object);

            _controller = new RobotController(robotRepository, loggerMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetAll_ReturnsAllRobots()
        {
            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var actionResult = result.Result as OkObjectResult;
            var robots = actionResult.Value as IEnumerable<RobotResponse>;
            Assert.IsNotNull(robots);
            Assert.IsTrue(robots.Any()); // Stellen Sie sicher, dass Testdaten vorhanden sind
        }

        [TestMethod]
        public async Task Get_ReturnsRobotById_WhenRobotExists()
        {
            var players = await _playerRepository.GetAllAsync();

            // Arrange
            var newRobotRequest = new RobotRequest
            {
                CurrentPlayerId = players.First().Id,
                Name ="Röbby",
                Color = "Red",
            };

            var postresult = await _controller.Post(newRobotRequest);

            var actionPostResult = postresult.Result as CreatedAtActionResult;
            var createdRobot = actionPostResult.Value as RobotResponse;
            var id = createdRobot.Id;
            // Act
            var result = await _controller.GetById(id);

            // Assert
            //Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var actionResult = result.Result as OkObjectResult;
            var robot = actionResult.Value as RobotResponse;
            Assert.IsNotNull(robot);
            Assert.AreEqual(id, robot.Id);
        }

        [TestMethod]
        public async Task Post_CreatesNewRobot()
        {
            var players = await _playerRepository.GetAllAsync();

            // Arrange
            var newRobotRequest = new RobotRequest
            {
                CurrentPlayerId = players.First().Id,
                Name = "Robot1",
                Color = "Red",
                IsConnected = false,
                IsIngame = false
            };

            // Act
            var result = await _controller.Post(newRobotRequest);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var actionResult = result.Result as CreatedAtActionResult;
            var createdRobot = actionResult.Value as RobotResponse;
            Assert.IsNotNull(createdRobot);

        }

        [TestMethod]
        public async Task Put_UpdatesRobot_WhenRobotExists()
        {
            var players = await _playerRepository.GetAllAsync();
            var player = players.First();
            // Arrange
            var newRobotRequest = new RobotRequest
            {
                CurrentPlayerId = player.Id,
                Name = "röbby",
                Color = "Red",
                IsIngame = false,
                IsConnected = false
            };

            // Act
            var postresult = await _controller.Post(newRobotRequest);
            var actionPostResult = postresult.Result as CreatedAtActionResult;
            var createdRobot = actionPostResult.Value as RobotResponse;

            var getByIdResult = await _controller.GetById(createdRobot.Id);
            var actionGetResult = getByIdResult.Result as OkObjectResult;
            var existingRobot = actionGetResult.Value as RobotResponse;

            var id = player.Id;
            player = players.First(p => p.Id != id);
            // Arrange
            var updatedRobotRequest = new RobotRequest
            {
                CurrentPlayerId = player.Id,
                Color = "Blue",
                Name = "foo",
                IsIngame = true,
                IsConnected = true
            };

            _context.SaveChanges();
            var result = await _controller.Put(existingRobot.Id, updatedRobotRequest);
            _context.SaveChanges();
            var updatedRobot = _context.Robots.Find(existingRobot.Id);
            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
                        
            Assert.IsNotNull(updatedRobot);
            Assert.AreEqual(player.Id, updatedRobot.CurrentPlayerId);
            Assert.AreEqual(ConnectFourColor.Blue, updatedRobot.Color);
            Assert.AreEqual(true, updatedRobot.IsConnected);
            Assert.AreEqual(true, updatedRobot.IsIngame);
            Assert.AreEqual("foo", updatedRobot.Name);

        }

        [TestMethod]
        public async Task Delete_DeletesRobot_WhenRobotExists()
        {

            // Arrange
            var newRobotRequest = new RobotRequest
            {
                CurrentPlayerId = null,
                Color = "Red",
                Name ="Röbby"
            };

            // Erstellen eines neuen Benutzers
            var postResult = await _controller.Post(newRobotRequest);
            var actionResult = postResult.Result as CreatedAtActionResult;
            var createdRobot = actionResult.Value as RobotResponse;
            var id = createdRobot.Id;
            _context.SaveChanges();

            var robot = _context.Robots.Find(id);
            Assert.IsNotNull(robot);
            Assert.IsNull(robot.DeletedOn);

            // Act
            var resultDelete = await _controller.Delete(id);
            _context.SaveChanges();

            // Assert
            Assert.IsInstanceOfType(resultDelete, typeof(NoContentResult));
            Assert.IsNotNull(robot.DeletedOn);

        }
    }
}

