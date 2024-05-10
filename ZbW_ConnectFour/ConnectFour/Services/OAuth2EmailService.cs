using Microsoft.AspNetCore.WebUtilities;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class OAuth2EmailService
{
	private readonly IEmailSender _emailSender;
	private readonly IUserRepository _userRepository;
	private readonly ILogger<OAuth2EmailService> _logger;

	public OAuth2EmailService(IEmailSender emailSender, IUserRepository userRepository, ILogger<OAuth2EmailService> logger)
	{
		_emailSender = emailSender;
		_userRepository = userRepository;
		_logger = logger;
	}

	public async Task<IActionResult> SendVerificationEmailAsync(string email)
	{
		var user = await _userRepository.FindByEmailAsync(email);
		if (user == null)
		{
			_logger.LogError($"No user found with email {email}");
			return new BadRequestObjectResult("User not found.");
		}

		if (user.VerificationToken == null)
		{
			user.VerificationToken = GenerateVerificationToken();
			await _userRepository.UpdateAsync(user);
		}

		var verificationUrl = BuildVerificationUrl(email, user.VerificationToken);
		var emailContent = BuildEmailContent(verificationUrl);

		var result = await _emailSender.SendEmailAsync(email, "Verify your email address", emailContent);
		if (!result)
		{
			_logger.LogError("Failed to send verification email.");
			return new StatusCodeResult(500);
		}

		_logger.LogInformation("Verification email sent successfully.");
		return new OkResult();
	}

	private string GenerateVerificationToken()
	{
		return Guid.NewGuid().ToString("N");
	}

	private string BuildVerificationUrl(string email, string token)
	{
		var queryParams = new Dictionary<string, string>
		{
			["token"] = token,
			["email"] = email
		};

		return QueryHelpers.AddQueryString("https://yourdomain.com/verify", queryParams);
	}

	private string BuildEmailContent(string verificationUrl)
	{
		return $"<html><body>Please verify your account by clicking <a href='{verificationUrl}'>here</a>.</body></html>";
	}
}
