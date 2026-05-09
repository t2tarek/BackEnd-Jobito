using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CompanyDashboardAPI.Services;

public interface IMailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
    Task SendPasswordResetCode(string toEmail, string code);
}

public class MailService : IMailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<MailService> _logger;

    public MailService(IConfiguration config, ILogger<MailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var host = _config["Smtp:Host"] ?? "smtp.gmail.com";
            var port = int.Parse(_config["Smtp:Port"] ?? "587");
            var user = _config["Smtp:User"] ?? "jobito.service@gmail.com";
            var pass = _config["Smtp:Pass"] ?? "password";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Jobito Platform", user));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(user, pass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email.");
        }
    }

    public async Task SendPasswordResetCode(string toEmail, string code)
    {
        var subject = "Your Jobito Password Reset Code";
        var body = $"<h1>Password Reset</h1><p>Your password reset code is: <strong>{code}</strong></p><p>This code will expire in 30 minutes.</p>";
        await SendEmailAsync(toEmail, subject, body);
    }
}
