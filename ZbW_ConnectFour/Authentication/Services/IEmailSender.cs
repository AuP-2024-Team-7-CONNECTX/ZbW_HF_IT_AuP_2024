using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using PostmarkDotNet;


namespace Authentication.Services;

public class EmailSender : IEmailSender
{
    private readonly ILogger _logger;

    public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor,
                       ILogger<EmailSender> logger)
    {
        Options = optionsAccessor.Value;
        _logger = logger;
    }

    public AuthMessageSenderOptions Options { get; } //Set with Secret Manager.

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        if (string.IsNullOrEmpty(Options.SendPostmarkKey))
        {
            throw new Exception("Null SendPostmarkKey");
        }
        await Execute(Options.SendPostmarkKey, subject, message, toEmail);
    }

    public async Task Execute(string apiKey, string subject, string message, string toEmail)
    {
        var client = new PostmarkClient("8600a7c6-16a7-4c4f-938e-e144b29f51de");

        // Send an email asynchronously:
        var postmarkMessage = new PostmarkMessage()
        {
            To = toEmail,
            From = "nick.ponnadu.gmx.ch@zbw-online.ch",
            TrackOpens = true,
            Subject = subject,
            TextBody = message,
            HtmlBody = message,
            MessageStream = "outbound",
            
        };
        var sendResult = await client.SendMessageAsync(postmarkMessage);

        if (sendResult.Status == PostmarkStatus.Success) { _logger.LogInformation("Success on sending Email"); }
        else { _logger.LogError("Error on sending Email"); }
    }
}