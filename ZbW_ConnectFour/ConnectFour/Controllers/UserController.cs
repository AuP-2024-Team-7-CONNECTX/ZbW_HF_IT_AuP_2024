using ConnectFour.Api.User;
using ConnectFour.Models;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConnectFour.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserRepository repository, ILogger<UserController> logger)
        {
            ArgumentNullException.ThrowIfNull(repository, nameof(repository));
            _repository = repository;
            _logger = logger;
        }


        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
        {
            try
            {
                var users = await _repository.GetAllAsync();
                var userResponses = users.Select(x => new UserResponse(x.Id, x.Name, x.Email, x.Password, x.Authenticated));
                return Ok(userResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all users.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET api/Users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> Get(string id)
        {
            try
            {
                var user = await _repository.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var userResponse = new UserResponse(user.Id, user.Name, user.Email, user.Password, user.Authenticated);
                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging purposes
                _logger.LogError(ex, "An error occurred while fetching the user with ID {UserId}.", id);

                // Return a generic error message to the client
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST api/Users
        [HttpPost]
        public async Task<ActionResult<UserResponse>> Post(UserRequest value)
        {
            try
            {
                var user = new User
                {
                    Id = value.Id,
                    Name = value.Name,
                    Email = value.Email,
                    Password = value.Password,
                    Authenticated = value.Authenticated
                };
                await _repository.CreateOrUpdateAsync(user);
                return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new user.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/Users/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(string id, UserRequest value)
        {
            var existingUser = await _repository.GetByIdAsync(id);

            if (existingUser != null)
            {

                try
                {

                    existingUser.Name = value.Name;
                    existingUser.Email = value.Email;
                    existingUser.Authenticated = value.Authenticated;
                    existingUser.Password = value.Password;

                    await _repository.CreateOrUpdateAsync(existingUser);
                    return NoContent();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating the user with ID {UserId}.", id);
                    return StatusCode(500, "An error occurred while processing your request.");
                }

            }
            else
            {
                return StatusCode(500, "No User found with this id");

            }
        }

        // DELETE api/Users/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                await _repository.DeleteAsync<User>(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the user with ID {UserId}.", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
