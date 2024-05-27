using PostmarkDotNet;
using System.Threading.Tasks;

public class PostmarkEmailSender : IEmailSender
{
	private readonly string _serverToken;
	private readonly string _fromEmail;

	public PostmarkEmailSender(string serverToken, string fromEmail)
	{
		_serverToken = serverToken;
		_fromEmail = fromEmail;
	}

	public async Task<bool> SendEmailAsync(string to, string subject, string htmlContent)
	{
		var client = new PostmarkClient(_serverToken);
		var message = new PostmarkMessage
		{
			To = to,
			From = _fromEmail,
			Subject = subject,
			HtmlBody = htmlContent,
			TrackOpens = true
		};

		try
		{
			var response = await client.SendMessageAsync(message);
			return response.Status == PostmarkStatus.Success;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		
	}
}
