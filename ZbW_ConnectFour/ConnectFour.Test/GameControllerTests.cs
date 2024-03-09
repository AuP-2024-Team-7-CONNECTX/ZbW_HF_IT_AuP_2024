using ConnectFour.Controllers;
using ConnectFour.Models;
using ConnectFour.Repositories.Implementations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ConnectFour.Enums.Enum;

namespace ConnectFour.Tests
{
    [TestClass]
    public class GameControllerTests
    {
        private GameDbContext _context;
        private GameController _controller;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new GameDbContext(options);

            // Hinzufügen von Testdaten
            var testGame = new Game
            {
                Players = new List<Player>
                {
            new Player { /* Initialisieren Sie das Player-Objekt entsprechend */ },
            new Player { /* Initialisieren Sie ein weiteres Player-Objekt entsprechend */ }
                    },
                Robots = new List<Robot>
                {
            new Robot { /* Initialisieren Sie das Robot-Objekt entsprechend */ }
                },
                State = GameState.Active,
                CurrentMove = null, // oder ein gültiger Move, wenn definiert
                WinnerPlayer = null, // Kann gesetzt werden, wenn ein Gewinner existiert
                TotalPointsPlayerOne = 100, // Beispielwert
                TotalPointsPlayerTwo = 95 // Beispielwert
            };

            _context.Games.Add(testGame);
            _context.SaveChanges();

            var loggerMock = new Mock<ILogger<GameController>>();
            var gameRepository = new GameRepository(new GenericRepository(_context, new Mock<ILogger<GenericRepository>>().Object));

            _controller = new GameController(gameRepository, loggerMock.Object);
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
            // Arrange - Hinzufügen von Testspielen in die In-Memory-Datenbank

            // Act
            var result = await _controller.GetAll(); // Implementieren Sie die Methode, falls sie noch nicht existiert

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var actionResult = result.Result as OkObjectResult;
            var games = actionResult.Value as IEnumerable<GameResponse>;
            Assert.IsNotNull(games);
            Assert.IsTrue(games.Any());
        }

        [TestMethod]
        public async Task GetGameById_ReturnsGame_WhenGameExists()
        {
            // Arrange
            var testGame = new Game
            {
                // Initialisieren Sie das Spielobjekt
            };
            await _context.Games.AddAsync(testGame);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Get(testGame.Id);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var actionResult = result.Result as OkObjectResult;
            var gameResponse = actionResult.Value as GameResponse;
            Assert.IsNotNull(gameResponse);
            Assert.AreEqual(testGame.Id, gameResponse.Id);
        }

        [TestMethod]
        public async Task CreateGame_CreatesNewGame()
        {
            // Arrange
            var newGame = new Game
            {
                // Initialisieren Sie Ihr Spielobjekt hier entsprechend
            };

            // Act
            var result = await _controller.Post(newGame);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var actionResult = result.Result as CreatedAtActionResult;
            var createdGame = actionResult.Value as Game;
            Assert.IsNotNull(createdGame);
            // Weitere Überprüfungen, um sicherzustellen, dass das Spiel korrekt erstellt wurde
        }

        [TestMethod]
        public async Task UpdateGame_UpdatesExistingGame()
        {
            // Arrange
            var existingGame = new Game
            {
                // Initialisieren Sie das bestehende Spielobjekt
            };
            _context.Games.Add(existingGame);
            await _context.SaveChangesAsync();

            var updatedGame = new Game
            {
                // Aktualisierte Spielinformationen
                Id = existingGame.Id // Stellen Sie sicher, dass die ID übereinstimmt
            };

            // Act
            var result = await _controller.Put(existingGame.Id, updatedGame);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            var dbGame = await _context.Games.FindAsync(existingGame.Id);
            // Überprüfen Sie hier die aktualisierten Eigenschaften
        }

        [TestMethod]
        public async Task DeleteGame_DeletesExistingGame()
        {
            // Arrange
            var gameToDelete = new Game
            {
                // Initialisieren Sie das Spielobjekt zum Löschen
            };
            _context.Games.Add(gameToDelete);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Delete(gameToDelete.Id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            var deletedGame = await _context.Games.FindAsync(gameToDelete.Id);
            Assert.IsNull(deletedGame);
        }
    }
}
