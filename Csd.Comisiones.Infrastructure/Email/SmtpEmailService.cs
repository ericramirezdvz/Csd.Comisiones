using Csd.Comisiones.Application.Common.Models;
using Csd.Comisiones.Application.Contracts.Infrastructure;
using Csd.Comisiones.Application.Features.Proveedores.SendProveedores;
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
            int autorizadorId,
            string correo,
            string folio,
            string obra,
            DateTime fechaInicio,
            DateTime fechaFin,
            List<EmpleadoEmailDto> empleados)
        {
            var template = EmailTemplateHelper.LoadTemplate("SolicitudPendienteEmpleados.html");

            var baseUrl = _settings.BaseUrl;

            var urlAprobar = $"{baseUrl}/api/solicitudes/{solicitudId}/aprobar";
            var urlRechazar = $"{baseUrl}/api/solicitudes/{solicitudId}/rechazar-form?autorizadorId={autorizadorId}";

            var empleadosRows = string.Join("", empleados.Select(e => $@"
                <tr style='text-align:center; border-bottom:1px solid #eee;'>
                    <td>{e.Nombre}</td>
                    <td>{e.FechaInicio:dd/MM/yyyy}</td>
                    <td>{e.FechaFin:dd/MM/yyyy}</td>
                    <td>{(e.RequiereHotel ? "✔" : "")}</td>
                    <td>{(e.Desayuno ? "✔" : "")}</td>
                    <td>{(e.Almuerzo ? "✔" : "")}</td>
                    <td>{(e.Cena ? "✔" : "")}</td>
                    <td><strong>${e.Total:N2}</strong></td>
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

        public async Task SendSolicitudAprobadaAsync(
            string correo,
            string folio,
            string obra,
            DateTime fechaInicio,
            DateTime fechaFin,
            List<EmpleadoEmailDto> empleados)
        {
            var template = EmailTemplateHelper.LoadTemplate("SolicitudAprobadaEmpleados.html");

            var empleadosRows = string.Join("", empleados.Select(e => $@"
                <tr style='text-align:center; border-bottom:1px solid #eee;'>
                    <td>{e.Nombre}</td>
                    <td>{e.FechaInicio:dd/MM/yyyy}</td>
                    <td>{e.FechaFin:dd/MM/yyyy}</td>
                    <td>{(e.RequiereHotel ? "✔" : "")}</td>
                    <td>{(e.Desayuno ? "✔" : "")}</td>
                    <td>{(e.Almuerzo ? "✔" : "")}</td>
                    <td>{(e.Cena ? "✔" : "")}</td>
                    <td><strong>${e.Total:N2}</strong></td>
                </tr>"));

            var body = EmailTemplateHelper.Replace(template, new Dictionary<string, string>
            {
                { "Folio", folio },
                { "Obra", obra },
                { "FechaInicio", fechaInicio.ToString("dd/MM/yyyy") },
                { "FechaFin", fechaFin.ToString("dd/MM/yyyy") },
                { "EmpleadosRows", empleadosRows }
            });

            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(correo));
            email.Subject = $"Solicitud {folio} aprobada";

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

        public async Task SendSolicitudRechazadaAsync(
            string correo,
            string folio,
            string obra,
            DateTime fechaInicio,
            DateTime fechaFin,
            List<EmpleadoEmailDto> empleados,
            string? motivo)
        {
            var template = EmailTemplateHelper.LoadTemplate("SolicitudRechazadaEmpleados.html");

            var empleadosRows = string.Join("", empleados.Select(e => $@"
                <tr style='text-align:center; border-bottom:1px solid #eee;'>
                    <td>{e.Nombre}</td>
                    <td>{e.FechaInicio:dd/MM/yyyy}</td>
                    <td>{e.FechaFin:dd/MM/yyyy}</td>
                    <td>{(e.RequiereHotel ? "✔" : "")}</td>
                    <td>{(e.Desayuno ? "✔" : "")}</td>
                    <td>{(e.Almuerzo ? "✔" : "")}</td>
                    <td>{(e.Cena ? "✔" : "")}</td>
                    <td><strong>${e.Total:N2}</strong></td>
                </tr>"));

            var body = EmailTemplateHelper.Replace(template, new Dictionary<string, string>
            {
                { "Folio", folio },
                { "Obra", obra },
                { "FechaInicio", fechaInicio.ToString("dd/MM/yyyy") },
                { "FechaFin", fechaFin.ToString("dd/MM/yyyy") },
                { "Motivo", motivo ?? "No especificado" },
                { "EmpleadosRows", empleadosRows }
            });

            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(correo));
            email.Subject = $"Solicitud {folio} rechazada";

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

        public async Task SendSolicitudProveedorAsync(
    string correo,
    string proveedorNombre,
    string folio,
    List<ProveedorDetalleDto> detalles)
        {
            var template = EmailTemplateHelper.LoadTemplate("SolicitudProveedor.html");

            var rows = string.Join("", detalles.Select(d => $@"
        <tr>
            <td>{d.NombreEmpleado}</td>
            <td>{d.TipoServicio}</td>
            <td>{d.FechaInicio:dd/MM/yyyy}</td>
            <td>{d.FechaFin:dd/MM/yyyy}</td>
        </tr>"));

            var body = EmailTemplateHelper.Replace(template, new Dictionary<string, string>
    {
        { "Proveedor", proveedorNombre },
        { "Folio", folio },
        { "Rows", rows }
    });

            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(correo));
            email.Subject = $"Nueva solicitud de servicio - {folio}";

            var builder = new BodyBuilder { HtmlBody = body };
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
