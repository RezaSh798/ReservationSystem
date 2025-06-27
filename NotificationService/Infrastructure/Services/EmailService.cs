using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using NotificationService.Application.Services;
using Microsoft.Extensions.Options;

namespace NotificationService.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendAsync(string to, string subject, string htmlMessage)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_settings.From));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}