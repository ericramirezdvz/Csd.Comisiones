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

        /// <summary>
        /// Genera la fila HTML de un empleado en la tabla del correo.
        /// Si es pago directo, muestra "Pago directo" en las columnas de servicios.
        /// </summary>
        private static string RenderEmpleadoRow(EmpleadoEmailDto e)
        {
            if (e.EsPago)
            {
                return $@"
                <tr style='text-align:center; border-bottom:1px solid #eee;'>
                    <td>{e.Nombre}</td>
                    <td>{e.FechaInicio:dd/MM/yyyy}</td>
                    <td>{e.FechaFin:dd/MM/yyyy}</td>
                    <td colspan='4' style='color:#f57f17; font-weight:600;'>Pago directo</td>
                    <td><strong>${e.Total:N2}</strong></td>
                </tr>";
            }

            return $@"
                <tr style='text-align:center; border-bottom:1px solid #eee;'>
                    <td>{e.Nombre}</td>
                    <td>{e.FechaInicio:dd/MM/yyyy}</td>
                    <td>{e.FechaFin:dd/MM/yyyy}</td>
                    <td>{(e.RequiereHotel ? "✔" : "")}</td>
                    <td>{(e.Desayuno ? "✔" : "")}</td>
                    <td>{(e.Almuerzo ? "✔" : "")}</td>
                    <td>{(e.Cena ? "✔" : "")}</td>
                    <td><strong>${e.Total:N2}</strong></td>
                </tr>";
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

            var empleadosRows = string.Join("", empleados.Select(RenderEmpleadoRow));


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

            var empleadosRows = string.Join("", empleados.Select(RenderEmpleadoRow));

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

            var empleadosRows = string.Join("", empleados.Select(RenderEmpleadoRow));

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
    string subFolio,
    List<ProveedorDetalleDto> detalles,
    Guid token,
    bool esConciliacion = false)
        {
            var templateName = esConciliacion
                ? "SolicitudProveedor.html"
                : "SolicitudProveedorInicial.html";
            var template = EmailTemplateHelper.LoadTemplate(templateName);

            var baseUrl = _settings.BaseUrl;
            var urlAceptar = $"{baseUrl}/api/proveedores/respuesta/{token}/aceptar";
            var urlRechazar = $"{baseUrl}/api/proveedores/respuesta/{token}/rechazar-form";

            // Filas de servicios agrupados con cantidad y subtotal
            var rows = string.Join("", detalles.Select(d =>
            {
                var empleadosList = d.Empleados.Count > 0
                    ? $"<div style='font-size:11px; color:#666; margin-top:2px;'>{string.Join(", ", d.Empleados)}</div>"
                    : "";

                // Para hospedaje/alimentos, mostrar días del periodo
                var diasInfo = d.Dias > 1
                    ? $"<div style='font-size:11px; color:#666;'>{d.Dias} días</div>"
                    : "";

                return $@"
        <tr style='border-bottom: 1px solid #eee;'>
            <td style='padding: 8px;'>{d.TipoServicio}{diasInfo}{empleadosList}</td>
            <td style='padding: 8px; text-align: center;'>{d.Cantidad}</td>
            <td style='padding: 8px; text-align: center;'>{d.FechaInicio:dd/MM/yyyy}</td>
            <td style='padding: 8px; text-align: center;'>{d.FechaFin:dd/MM/yyyy}</td>
            <td style='padding: 8px; text-align: right;'>{d.PrecioUnitario:C2}</td>
            <td style='padding: 8px; text-align: right; font-weight: 600;'>{d.Subtotal:C2}</td>
        </tr>";
            }));

            // Fila de total
            var total = detalles.Sum(d => d.Subtotal);
            rows += $@"
        <tr style='background: #f0f0f0; font-weight: bold;'>
            <td colspan='5' style='padding: 10px 8px; text-align: right;'>TOTAL</td>
            <td style='padding: 10px 8px; text-align: right; font-size: 15px;'>{total:C2}</td>
        </tr>";

            var body = EmailTemplateHelper.Replace(template, new Dictionary<string, string>
    {
        { "Proveedor", proveedorNombre },
        { "Folio", folio },
        { "SubFolio", subFolio },
        { "Rows", rows },
        { "UrlAceptar", urlAceptar },
        { "UrlRechazar", urlRechazar }
    });

            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(correo));
            email.Subject = esConciliacion
                ? $"Conciliación de servicios - {subFolio}"
                : $"Solicitud de servicios - {subFolio}";

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_settings.Server, _settings.Port,
                _settings.UseSSL ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);

            await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendProveedorRechazoNotificacionAsync(
            string folio,
            string proveedorNombre,
            string motivo)
        {
            var template = EmailTemplateHelper.LoadTemplate("ProveedorRechazoNotificacion.html");

            var body = EmailTemplateHelper.Replace(template, new Dictionary<string, string>
            {
                { "Proveedor", proveedorNombre },
                { "Folio", folio },
                { "Motivo", motivo }
            });

            var correoDestino = _settings.SenderEmail;

            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(correoDestino));
            email.Subject = $"⚠️ Proveedor rechazó servicios - {folio}";

            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_settings.Server, _settings.Port,
                _settings.UseSSL ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);

            await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendSolpedAsync(
            string folio,
            string obra,
            string area,
            string periodo,
            string tablaAlimentacion,
            string tablaHospedaje,
            byte[]? excelAdjunto = null)
        {
            var destinatarios = _settings.SolpedRecipients;
            if (destinatarios == null || destinatarios.Length == 0)
                throw new InvalidOperationException("No hay destinatarios configurados para Solpeds (SmtpSettings:SolpedRecipients).");

            var template = EmailTemplateHelper.LoadTemplate("SolpedControlFormato.html");

            var body = EmailTemplateHelper.Replace(template, new Dictionary<string, string>
            {
                { "Folio", folio },
                { "Obra", obra },
                { "Area", area },
                { "Periodo", periodo },
                { "TablaAlimentacion", tablaAlimentacion },
                { "TablaHospedaje", tablaHospedaje }
            });

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));

            foreach (var correo in destinatarios)
                email.To.Add(MailboxAddress.Parse(correo));

            email.Subject = $"Control de Alimentación y Hospedaje - {folio}";

            var builder = new BodyBuilder { HtmlBody = body };

            if (excelAdjunto != null && excelAdjunto.Length > 0)
            {
                builder.Attachments.Add(
                    $"Solped_{folio}.xlsx",
                    excelAdjunto,
                    new MimeKit.ContentType("application", "vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            }

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_settings.Server, _settings.Port,
                _settings.UseSSL ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
            await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendGeneradorProveedorAsync(
            string correoProveedor,
            string proveedorNombre,
            string folio,
            byte[] excelAdjunto)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(correoProveedor));
            email.Subject = $"Generador de Servicios - {folio} - {proveedorNombre}";

            var builder = new BodyBuilder
            {
                HtmlBody = $@"
                    <p>Estimado(a) <strong>{proveedorNombre}</strong>,</p>
                    <p>Se adjunta el generador de servicios y subcontratos correspondiente al folio <strong>{folio}</strong>.</p>
                    <p>Favor de revisar la información y confirmar la recepción.</p>
                    <br/>
                    <p style='color:#888; font-size:12px;'>Este es un correo generado automáticamente por el sistema de comisiones DIAVAZ.</p>"
            };

            builder.Attachments.Add(
                $"Generador_{folio}_{proveedorNombre.Replace(" ", "_")}.xlsx",
                excelAdjunto,
                new MimeKit.ContentType("application", "vnd.openxmlformats-officedocument.spreadsheetml.sheet"));

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_settings.Server, _settings.Port,
                _settings.UseSSL ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
            await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendSolicitudProveedorModificadaAsync(
            string correo,
            string proveedorNombre,
            string folio,
            string empleadoNombre,
            List<ProveedorDetalleDto> eliminados,
            List<ProveedorDetalleDto> vigentes,
            Guid token)
        {
            var template = EmailTemplateHelper.LoadTemplate("SolicitudProveedorModificacion.html");

            var baseUrl = _settings.BaseUrl;
            var urlAceptar = $"{baseUrl}/api/proveedores/respuesta/{token}/aceptar";
            var urlRechazar = $"{baseUrl}/api/proveedores/respuesta/{token}/rechazar-form";

            // Build "Cancelados" section — servicios que se quitaron
            var seccionCancelados = "";
            if (eliminados.Count > 0)
            {
                var rows = string.Join("", eliminados.Select(d => $@"
                    <tr style='border-bottom: 1px solid #eee;'>
                        <td style='padding: 8px; text-decoration: line-through; color: #999;'>{d.TipoServicio}</td>
                        <td style='padding: 8px; text-align: center;'>{d.Dias}</td>
                        <td style='padding: 8px; text-align: center; text-decoration: line-through; color: #999;'>{d.FechaInicio:dd/MM/yyyy}</td>
                        <td style='padding: 8px; text-align: center; text-decoration: line-through; color: #999;'>{d.FechaFin:dd/MM/yyyy}</td>
                        <td style='padding: 8px; text-align: right; text-decoration: line-through; color: #999;'>{d.PrecioUnitario:C2}</td>
                        <td style='padding: 8px; text-align: right; text-decoration: line-through; color: #999;'>{d.Subtotal:C2}</td>
                    </tr>"));

                seccionCancelados = $@"
                <h3 style='color: #c62828; margin-top: 20px;'>&#10060; Servicios cancelados</h3>
                <table width='100%' cellpadding='8' cellspacing='0'
                    style='border-collapse: collapse; margin: 10px 0; font-size: 14px;'>
                    <thead>
                        <tr style='background: #ffebee;'>
                            <th style='text-align: left; border-bottom: 2px solid #ef9a9a; padding: 10px 8px;'>Servicio</th>
                            <th style='text-align: center; border-bottom: 2px solid #ef9a9a; padding: 10px 8px;'>Días</th>
                            <th style='text-align: center; border-bottom: 2px solid #ef9a9a; padding: 10px 8px;'>Inicio</th>
                            <th style='text-align: center; border-bottom: 2px solid #ef9a9a; padding: 10px 8px;'>Fin</th>
                            <th style='text-align: right; border-bottom: 2px solid #ef9a9a; padding: 10px 8px;'>P. Unit.</th>
                            <th style='text-align: right; border-bottom: 2px solid #ef9a9a; padding: 10px 8px;'>Subtotal</th>
                        </tr>
                    </thead>
                    <tbody>{rows}</tbody>
                </table>";
            }

            // Build "Vigentes" section — servicios que permanecen activos
            var seccionVigentes = "";
            if (vigentes.Count > 0)
            {
                var rows = string.Join("", vigentes.Select(d =>
                {
                    var diasInfo = d.Dias > 1
                        ? $"<div style='font-size:11px; color:#666;'>{d.Dias} días</div>"
                        : "";
                    return $@"
                    <tr style='border-bottom: 1px solid #eee;'>
                        <td style='padding: 8px;'>{d.TipoServicio}{diasInfo}</td>
                        <td style='padding: 8px; text-align: center;'>{d.Cantidad}</td>
                        <td style='padding: 8px; text-align: center;'>{d.FechaInicio:dd/MM/yyyy}</td>
                        <td style='padding: 8px; text-align: center;'>{d.FechaFin:dd/MM/yyyy}</td>
                        <td style='padding: 8px; text-align: right;'>{d.PrecioUnitario:C2}</td>
                        <td style='padding: 8px; text-align: right; font-weight: 600;'>{d.Subtotal:C2}</td>
                    </tr>";
                }));

                var total = vigentes.Sum(d => d.Subtotal);
                rows += $@"
                    <tr style='background: #f0f0f0; font-weight: bold;'>
                        <td colspan='5' style='padding: 10px 8px; text-align: right;'>TOTAL VIGENTE</td>
                        <td style='padding: 10px 8px; text-align: right; font-size: 15px;'>{total:C2}</td>
                    </tr>";

                seccionVigentes = $@"
                <h3 style='color: #1565c0; margin-top: 20px;'>&#9989; Servicios vigentes</h3>
                <table width='100%' cellpadding='8' cellspacing='0'
                    style='border-collapse: collapse; margin: 10px 0; font-size: 14px;'>
                    <thead>
                        <tr style='background: #e3f2fd;'>
                            <th style='text-align: left; border-bottom: 2px solid #90caf9; padding: 10px 8px;'>Servicio</th>
                            <th style='text-align: center; border-bottom: 2px solid #90caf9; padding: 10px 8px;'>Cant.</th>
                            <th style='text-align: center; border-bottom: 2px solid #90caf9; padding: 10px 8px;'>Inicio</th>
                            <th style='text-align: center; border-bottom: 2px solid #90caf9; padding: 10px 8px;'>Fin</th>
                            <th style='text-align: right; border-bottom: 2px solid #90caf9; padding: 10px 8px;'>P. Unit.</th>
                            <th style='text-align: right; border-bottom: 2px solid #90caf9; padding: 10px 8px;'>Subtotal</th>
                        </tr>
                    </thead>
                    <tbody>{rows}</tbody>
                </table>";
            }
            else
            {
                seccionVigentes = @"
                <div style='background: #fff3e0; border-left: 4px solid #e65100; padding: 12px 16px; margin-top: 20px; font-size: 14px;'>
                    <strong>&#9888; Todos los servicios han sido cancelados</strong> para este empleado.
                </div>";
            }

            var body = EmailTemplateHelper.Replace(template, new Dictionary<string, string>
            {
                { "Proveedor", proveedorNombre },
                { "Folio", folio },
                { "Empleado", empleadoNombre },
                { "SeccionCancelados", seccionCancelados },
                { "SeccionVigentes", seccionVigentes },
                { "UrlAceptar", urlAceptar },
                { "UrlRechazar", urlRechazar }
            });

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(correo));
            email.Subject = $"⚠️ Modificación de servicios - {folio}";

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
