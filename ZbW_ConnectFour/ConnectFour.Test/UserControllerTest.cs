using ConnectFour.Api.User;
using ConnectFour.Controllers;
using ConnectFour.Models;
using ConnectFour.Repositories.Implementations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ConnectFour.Tests
{
    [TestClass]
    public class UserControllerTests
    {
        private GameDbContext _context;
        private UserController _controller;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new GameDbContext(options);
            TestConfigurationHelper.InitializeDbForTests(_context);

            var loggerMock = new Mock<ILogger<UserController>>();
            var userRepository = new UserRepository(new GenericRepository(_context, new Mock<ILogger<GenericRepository>>().Object));

            _controller = new UserController(userRepository, loggerMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetAll_ReturnsAllUsers()
        {
            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var actionResult = result.Result as OkObjectResult;
            var users = actionResult.Value as IEnumerable<UserResponse>;
            Assert.IsTrue(users.Any());
            Assert.AreEqual(2, users.Count()); // Gelöschte werden nicht geholt

        }

        [TestMethod]
        public async Task GetById_ReturnsOneUser()
        {
            // Arrange
            var newUserRequest = new UserRequest("Test User", "test@example.com", false);

            // Act
            var postResult = await _controller.Post(newUserRequest);
            var actionPostResult = postResult.Result as CreatedAtActionResult;
            var newUser = actionPostResult.Value as User;

            var result = await _controller.Get(newUser.Id);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var actionResult = result.Result as OkObjectResult;
            var user = actionResult.Value as UserResponse;
            Assert.IsNotNull(user);
        }

        [TestMethod]
        public async Task Get_ReturnsUserById_WhenUserExists()
        {
            // Arrange
            var testUserId = _context.Users.First().Id.ToString();

            // Act
            var result = await _controller.Get(testUserId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var actionResult = result.Result as OkObjectResult;
            var user = actionResult.Value as UserResponse;
            Assert.AreEqual(testUserId, user.Id);
        }

        [TestMethod]
        public async Task Post_CreatesNewUser()
        {
            // Arrange
            var newUserRequest = new UserRequest("Test User", "test@example.com", false);

            // Act
            var postResult = await _controller.Post(newUserRequest);
            Assert.IsInstanceOfType(postResult.Result, typeof(CreatedAtActionResult));

            var actionPostResult = postResult.Result as CreatedAtActionResult;
            var newUser = actionPostResult.Value as User;

            newUser = _context.Users.Find(newUser.Id);

            // Assert
            Assert.IsNotNull(newUser);

        }

        [TestMethod]
        public async Task Put_UpdatesUser_WhenUserExists()
        {
            // Arrange
            var existingUser = _context.Users.First();
            var updateUserRequest = new UserRequest("Updated Name", "updated@example.com", true);

            // Act
            var result = await _controller.Put(existingUser.Id, updateUserRequest);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _context.Entry(existingUser).Reload();
            Assert.AreEqual("Updated Name", existingUser.Name);
            Assert.AreEqual("updated@example.com", existingUser.Email);
            Assert.IsTrue(existingUser.Authenticated);
        }


        [TestMethod]
        public async Task Delete_MarksUserAsDeleted_WhenUserExists()
        {
            // Arrange
            var newUserRequest = new UserRequest("Test User", "test@example.com", false);

            // Erstellen eines neuen Benutzers
            var postResult = await _controller.Post(newUserRequest);
            var actionPostResult = postResult.Result as CreatedAtActionResult;
            var newUser = actionPostResult.Value as User;
            var id = newUser.Id;
            _context.SaveChanges();

            var user = _context.Users.Find(id);
            Assert.IsNotNull(user);
            Assert.IsNull(user.DeletedOn);

            // Act
            var resultDelete = await _controller.Delete(id);
            _context.SaveChanges();


            // Assert
            Assert.IsInstanceOfType(resultDelete, typeof(NoContentResult));
            Assert.IsNotNull(user.DeletedOn);
        }


    }
}
