using Csd.Comisiones.Application.Common.Models;
using Csd.Comisiones.Application.Contracts.Infrastructure;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;


namespace Csd.Comisiones.Infrastructure.Email
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpSettings _settings;

        public SmtpEmailService(IOptions<SmtpSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendAsync(EmailMessage message)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(
                _settings.SenderName,
                _settings.SenderEmail));

            foreach (var to in message.To)
            {
                email.To.Add(MailboxAddress.Parse(to));
            }

            email.Subject = message.Subject;

            var builder = new BodyBuilder
            {
                HtmlBody = message.IsHtml ? message.Body : null,
                TextBody = message.IsHtml ? null : message.Body
            };

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _settings.Server,
                _settings.Port,
                _settings.UseSSL ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto
            );

            await smtp.AuthenticateAsync(
                _settings.Username,
                _settings.Password
            );

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }
    }
}
