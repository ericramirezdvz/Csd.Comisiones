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

        public async Task SendSolicitudPendienteAsync(
        string correo,
        string folio,
        string obra,
        DateTime fechaInicio,
        DateTime fechaFin)
        {
            var template = EmailTemplateHelper.LoadTemplate("SolicitudPendiente.html");

            var body = EmailTemplateHelper.Replace(template, new Dictionary<string, string>
        {
            { "Folio", folio },
            { "Obra", obra },
            { "FechaInicio", fechaInicio.ToString("dd/MM/yyyy") },
            { "FechaFin", fechaFin.ToString("dd/MM/yyyy") }
        });

            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(correo));
            email.Subject = "Solicitud pendiente de autorización";

            var builder = new BodyBuilder
            {
                HtmlBody = body
            };

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_settings.Server, _settings.Port,
                _settings.UseSSL ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);

            await smtp.AuthenticateAsync(_settings.Username, _settings.Password);

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }
    }
}
