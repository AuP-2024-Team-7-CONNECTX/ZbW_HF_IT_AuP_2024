public interface IEmailSender
{
	Task<bool> SendEmailAsync(string to, string subject, string htmlContent);
}
