using ConnectFour.Api.User;
using ConnectFour.Models;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConnectFour.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class UserController : ControllerBase
	{
		private readonly IUserRepository _repository;
		private readonly ILogger<UserController> _logger;
		private readonly ITokenService _tokenService;
		private readonly IEmailSender _emailService;

		public UserController(IUserRepository repository, ILogger<UserController> logger, ITokenService tokenService, IEmailSender emailService)
		{
			ArgumentNullException.ThrowIfNull(repository, nameof(repository));
			_repository = repository;
			_logger = logger;
			_tokenService = tokenService;
			_emailService = emailService;
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
				return StatusCode(500, $"An error occurred while processing your request.{ex.Message}");
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
				return StatusCode(500, $"An error occurred while processing your request.{ex.Message}");
			}
		}

		// POST api/Users
		[HttpPost]
		public async Task<ActionResult<UserResponse>> Post(UserRequest value)
		{
			var users = await _repository.GetAllAsync();

			if (users.Any(u => u.Name == value.Name))
			{
				return StatusCode(500, $"User already exists");

			}

			try
			{
				var user = new User
				{
					Id = Guid.NewGuid().ToString(),
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
				return StatusCode(500, $"An error occurred while processing your request. {ex.Message}");
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
					return StatusCode(500, $"An error occurred while processing your request.{ex.Message}");
				}

			}
			else
			{
				return StatusCode(500, "No User found with this id");

			}
		}

		// DELETE /Users/{id}
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
				return StatusCode(500, $"An error occurred while processing your request.{ex.Message}");
			}
		}

		// POST: /Users/registeremail
		[HttpPost("registeremail")]
		public async Task<IActionResult> RegisterEmail(string email)
		{
			var users = await _repository.GetAllAsync();

			var user = users.FirstOrDefault(x => x.Email == email);
			if (user == null)
			{
				_logger.LogError($"No existing user with email {email}");
				return BadRequest("No existing user with this email address.");
			}
			if (user.Authenticated)
			{
				_logger.LogInformation($"User {user.Id} already authenticated.");
				return BadRequest("This user is already authenticated.");
			}

			await _repository.CreateOrUpdateAsync(user);

			var verificationUrl = $"https://connectx.mon3y.ch/Bestatigung/bestatigung.html";
			var emailBody = $"<html><body>Please confirm your account by <a href='{verificationUrl}'>clicking here</a>.</body></html>";

			var emailResult = await _emailService.SendEmailAsync(user.Email, "Confirm Your Email", emailBody);
			if (emailResult)
			{
				_logger.LogInformation("Verification email sent successfully.");
				return Ok("Verification email sent successfully. Please check your email to confirm.");
			}

			_logger.LogError("Failed to send verification email.");
			return StatusCode(500, "Failed to send verification email.");
		}


		// POST: /Users/confirmEmail
		[HttpPost("confirmEmail")]
		public async Task<IActionResult> ConfirmEmail(string email, string token)
		{
			try
			{
				var users = await _repository.GetAllAsync();

				var existingUser = users.FirstOrDefault(x => x.Email == email);

				if (existingUser == null)
				{
					_logger.LogError($"no existing user with mail {email}");
					return BadRequest($"no existing user with mail  {email}");
				}
				if (existingUser.Authenticated)
				{
					_logger.LogInformation($"user {existingUser.Id}, {existingUser.Name}, {existingUser.Email} already authenticated");
					return BadRequest($"user {existingUser.Id}, {existingUser.Name}, {existingUser.Email} already authenticated");
				}

				existingUser.Authenticated = true;

				await _repository.CreateOrUpdateAsync(existingUser);
				return Ok("Email confirmed. User can log in now");

			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// POST: /User/changepassword
		[HttpPost("changepassword")]
		public async Task<IActionResult> ChangePassword(string email, string newPassword)
		{
			try
			{
				var users = await _repository.GetAllAsync();

				var existingUser = users.FirstOrDefault(x => x.Email == email);

				if (existingUser == null)
				{
					_logger.LogError($"no existing user with mail {email}");
					return BadRequest($"no existing user with mail  {email}");
				}

				existingUser.Password = newPassword;
				await _repository.CreateOrUpdateAsync(existingUser);

				return Ok("Password changed.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}
	}
}
