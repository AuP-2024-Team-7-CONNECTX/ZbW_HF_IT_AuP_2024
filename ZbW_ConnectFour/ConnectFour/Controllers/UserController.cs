using ConnectFour.Api.User;
using ConnectFour.Models;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using PostmarkDotNet.Model;
using PostmarkDotNet;
using System.Text.Encodings.Web;
using Serilog;

namespace ConnectFour.Controllers
{
	[ApiController]
	[Route("[controller]")]
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
				return StatusCode(500, $"An error occurred while processing your request.{ex.Message}");
			}
		}

		// POST: /api/Users/registeremail
		[HttpPost("registeremail")]
		public async Task<IActionResult> RegisterEmail(string email)
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

			try
			{
				var client = new PostmarkClient("8600a7c6-16a7-4c4f-938e-e144b29f51de");

				string encodedUrl = HtmlEncoder.Default.Encode("https://connectx.tailf1dac2.ts.net/Bestatigung/bestatigung.html");
				string emailParam = $"?email={Uri.EscapeDataString(email)}"; // Uri.EscapeDataString sorgt für eine korrekte URL-Codierung

				var postmarkMessage = new PostmarkMessage()
				{
					To = email,
					From = "nick.ponnadu.gmx.ch@zbw-online.ch",
					TrackOpens = true,
					Subject = "ConnectX Registration: Please confirm your E-Mail",
					TextBody = $"Please confirm your account by clicking here: {encodedUrl}",
					HtmlBody = $"<html><body>Please confirm your account by <a href='{encodedUrl}'>clicking here</a>.</body></html>",
					MessageStream = "outbound",
				};

				var sendResult = await client.SendMessageAsync(postmarkMessage);

				if (sendResult.Status == PostmarkStatus.Success)
				{
					_logger.LogInformation("Success on sending Email");

					return Ok("Authentication was successful. Confirm email to register.");
				}
				else
				{
					_logger.LogError($"Error on sending Email: {sendResult.ErrorCode}");
					return BadRequest($"Failed to send email. {sendResult.ErrorCode}");
				}

			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		// POST: /api/Users/confirmEmail
		[HttpPost("confirmEmail")]
		public async Task<IActionResult> ConfirmEmail(string email)
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

		// POST: /api/authentication/changepassword
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
