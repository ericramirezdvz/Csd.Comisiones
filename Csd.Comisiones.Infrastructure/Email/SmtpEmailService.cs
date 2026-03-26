using Csd.Comisiones.Application.Common.Models;
using Csd.Comisiones.Application.Contracts.Infrastructure;
using Csd.Comisiones.Domain.Entities;
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
        int solicitudId,
        string correo,
        string folio,
        string obra,
        DateTime fechaInicio,
        DateTime fechaFin,
        List<EmpleadoEmailDto> empleados)
        {
            var template = EmailTemplateHelper.LoadTemplate("SolicitudPendienteEmpleados.html");

            var baseUrl = "http://localhost"; //_settings.BaseUrl;

            var urlAprobar = $"{baseUrl}/api/solicitudes/{solicitudId}/approve";
            var urlRechazar = $"{baseUrl}/api/solicitudes/{solicitudId}/reject";

            var empleadosRows = string.Join("", empleados.Select(e => $@"
                <tr style='text-align:center; border-bottom:1px solid #eee;'>
                    <td>{e.Nombre}</td>
                    <td>{e.FechaInicio:dd/MM/yyyy}</td>
                    <td>{e.FechaFin:dd/MM/yyyy}</td>
                    <td>{(e.RequiereHotel ? "✔" : "")}</td>
                    <td>{(e.Desayuno ? "✔" : "")}</td>
                    <td>{(e.Almuerzo ? "✔" : "")}</td>
                    <td>{(e.Cena ? "✔" : "")}</td>
                </tr>"));


            var body = EmailTemplateHelper.Replace(template, new Dictionary<string, string>
            {
                { "Folio", folio },
                { "Obra", obra },
                { "FechaInicio", fechaInicio.ToString("dd/MM/yyyy") },
                { "FechaFin", fechaFin.ToString("dd/MM/yyyy") },
                { "UrlAprobar", urlAprobar },
                { "UrlRechazar", urlRechazar },
                { "EmpleadosRows", empleadosRows }
            });

            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(correo));
            email.Subject = $"Solicitud {folio} pendiente de autorización";

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
