using ConnectFour.Api.User;
using ConnectFour.Models;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Generators;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;

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

		private JsonResponseMessage _responseJson;

		public UserController(IUserRepository repository, ILogger<UserController> logger, ITokenService tokenService, IEmailSender emailService)
		{
			ArgumentNullException.ThrowIfNull(repository, nameof(repository));
			_repository = repository;
			_logger = logger;
			_tokenService = tokenService;
			_emailService = emailService;

			_responseJson = new JsonResponseMessage();
		}


		// GET: api/Users
		[HttpGet]
		public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
		{
			try
			{
				var users = await _repository.GetAllAsync();
				var userResponses = users.Select(x => new UserResponse(x.Id, x.Name, x.Email, x.Password, x.Authenticated,x.IsIngame));
				return Ok(userResponses);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while fetching all users.");
				_responseJson.Message = $"{ex.Message}";
				return StatusCode(500, _responseJson);
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
					_responseJson.Message = $"Benutzer {id} konnte nicht gefunden werden.";
					return StatusCode(400, _responseJson);
				}

				var userResponse = new UserResponse(user.Id, user.Name, user.Email, user.Password, user.Authenticated,user.IsIngame);
				return Ok(userResponse);
			}
			catch (Exception ex)
			{
				// Log the exception details for debugging purposes
				_logger.LogError(ex, $"An error occurred while processing your request. {ex.Message}", id);
				_responseJson.Message = $"{ex.Message}";
				return StatusCode(500, _responseJson);
			}
		}

		// POST api/Users
		[HttpPost]
		public async Task<ActionResult<UserResponse>> Post(UserRequest value)
		{
			var users = await _repository.GetAllAsync();


			if (users.Any(u => u.Name == value.Name))
			{
				_responseJson.Message = "Benutzer existiert bereits";
				return StatusCode(500, _responseJson);

			}

			// password hashen, im frontend gleiche funktion dazu im frontend einbauen
			var hashedPassword = HashPassword(value.Password);

			try
			{
				var user = new User
				{
					Id = Guid.NewGuid().ToString(),
					Name = value.Name,
					Email = value.Email,
					Password = hashedPassword,
					Authenticated = value.Authenticated
				};
				await _repository.CreateOrUpdateAsync(user);
				return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while creating a new user.");
				_responseJson.Message = ex.Message;
				return StatusCode(500, _responseJson);

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

					existingUser.IsIngame = value.isIngame;

					await _repository.CreateOrUpdateAsync(existingUser);
					return NoContent();
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "An error occurred while updating the user with ID {UserId}.", id);
					_responseJson.Message = ex.Message;
					return StatusCode(500, _responseJson);
				}

			}
			else
			{
				_responseJson.Message = "Kein Benutzer mit dieser Id gefunden";
				return StatusCode(500, _responseJson);

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
				_responseJson.Message = $"Kein Benutzer mit Mail: {email} gefunden";
				return StatusCode(400, _responseJson);

			}
			if (user.Authenticated)
			{
				_responseJson.Message = $"User {user.Id} bereits verifiziert.";
				return StatusCode(400, _responseJson);
			}

			var verificationUrl = $"https://connectx.mon3y.ch/Bestatigung/bestatigung.html?email={Uri.EscapeDataString(email)}";

			var emailBody = $"<html><body>Please confirm your account by <a href='{verificationUrl}'>clicking here</a>.</body></html>";

			var emailResult = await _emailService.SendEmailAsync(user.Email, "Confirm Your Email", emailBody);
			if (emailResult)
			{
				_logger.LogInformation("Verification email sent successfully.");
				_responseJson.Message = "Verifizierungsmail erfolgreich versendet. Bitte prüfen sie ihren Posteingang";
				return StatusCode(200, _responseJson);
			}

			_responseJson.Message = "Fehler beim Mailversand. Bitte Logs oder Konsole überprüfen";
			return StatusCode(500, _responseJson);
		}


		// POST: /Users/confirmEmail
		[HttpPost("confirmEmail")]
		public async Task<IActionResult> ConfirmEmail(string email)
		{
			try
			{
				var users = await _repository.GetAllAsync();

				var existingUser = users.FirstOrDefault(x => x.Email == email);

				if (existingUser == null)
				{
					_responseJson.Message = $"Kein Benutzer mit Mail: {email} gefunden";
					return StatusCode(400, _responseJson);

				}
				if (existingUser.Authenticated)
				{
					_responseJson.Message = $"User {email} bereits verifiziert.";
					return StatusCode(400, _responseJson);
				}

				existingUser.Authenticated = true;

				await _repository.CreateOrUpdateAsync(existingUser);
				_responseJson.Message = $"Email wurde verifiziert. Sie können sich nun einloggen!";
				return StatusCode(200, _responseJson);

			}
			catch (Exception ex)
			{
				_responseJson.Message = ex.Message;
				return StatusCode(500, _responseJson);
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
					_responseJson.Message = $"Kein Benutzer mit Mail: {email} gefunden";
					return StatusCode(400, _responseJson);
				}

				var hashedPassword = HashPassword(newPassword);


				existingUser.Password = hashedPassword;
				await _repository.CreateOrUpdateAsync(existingUser);

				_responseJson.Message = "Passwort konnte erfolgreich geändert werden";
				return StatusCode(200, _responseJson);
			}
			catch (Exception ex)
			{
				_responseJson.Message = ex.Message;
				return StatusCode(500, _responseJson);
			}
		}


		private string HashPassword(string password)
		{
			using (SHA256 sha256Hash = SHA256.Create())
			{
				byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < bytes.Length; i++)
				{
					builder.Append(bytes[i].ToString("x2"));
				}
				return builder.ToString();
			}
		}
	}
}
