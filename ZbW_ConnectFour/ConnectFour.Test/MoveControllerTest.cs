using ConnectFour.Controllers;
using ConnectFour.Models;
using ConnectFour.Repositories.Implementations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ConnectFour.Enums.Enum;

namespace ConnectFour.Tests
{
    [TestClass]
    public class MoveControllerTests
    {
        private GameDbContext _context;
        private MoveController _controller;
        private MoveRepository _moveRepository;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new GameDbContext(options);
            TestConfigurationHelper.InitializeDbForTests(_context); // Ensure this helper method initializes your DB with necessary test data

            var loggerMock = new Mock<ILogger<MoveController>>();
            _moveRepository = new MoveRepository(new GenericRepository(_context, new Mock<ILogger<GenericRepository>>().Object));

            _controller = new MoveController(_moveRepository, loggerMock.Object);

            var testGame = CreateGame();

        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetAll_ReturnsAllMoves()
        {
            var result = await _controller.GetAll();
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var actionResult = result.Result as OkObjectResult;
            var moves = actionResult.Value as IEnumerable<MoveResponse>; // Ensure MoveResponse is defined
            Assert.IsTrue(moves.Any());
        }

        [TestMethod]
        public async Task GetById_ReturnsOneMove()
        {
            var testMoveId = _context.Moves.First().Id;

            var result = await _controller.Get(testMoveId);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var actionResult = result.Result as OkObjectResult;
            var move = actionResult.Value as MoveResponse; // Ensure MoveResponse is defined
            Assert.IsNotNull(move);
            Assert.AreEqual(testMoveId, move.Id);
        }

        [TestMethod]
        public async Task Post_CreatesNewMove()
        {
            var newGame = CreateGame();

            var newMove = new Move
            {
                // Initialize your move object here
            };

            var postResult = await _controller.Post(newMove);
            Assert.IsInstanceOfType(postResult.Result, typeof(CreatedAtActionResult));

            var actionPostResult = postResult.Result as CreatedAtActionResult;
            var createdMove = actionPostResult.Value as Move;
            Assert.IsNotNull(createdMove);
            // Further assertions to verify the move was created correctly
        }

        [TestMethod]
        public async Task Put_UpdatesExistingMove()
        {
            var existingMove = _context.Moves.First();
            var updatedMoveDetails = new Move
            {
                // Update details here
            };

            var result = await _controller.Put(existingMove.Id, updatedMoveDetails);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            // Reload and assert the changes were made
            _context.Entry(existingMove).Reload();
            // Assert changes
        }

        [TestMethod]
        public async Task Delete_DeletesMove()
        {
            var moveToDelete = _context.Moves.First();
            var result = await _controller.Delete(moveToDelete.Id);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            var deletedMove = _context.Moves.Find(moveToDelete.Id);
            Assert.IsNull(deletedMove);
        }

        private Game CreateGame()
        {

            var user = new User { Id = Guid.NewGuid().ToString(), Name = "TestUser", Authenticated = true, Email = "test" };
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
            return testGame;
        }
    }
}
