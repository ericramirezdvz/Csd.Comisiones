using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Application.Features.Proveedores.AceptarRespuesta;
using Csd.Comisiones.Application.Features.Proveedores.RechazarRespuesta;
using Csd.Comisiones.Infrastructure.Email;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Csd.Comisiones.Api.Controllers
{
    /// <summary>
    /// Endpoints públicos para que los proveedores acepten o rechacen servicios
    /// desde los links del correo electrónico (sin autenticación, basado en token).
    /// </summary>
    [Route("api/proveedores/respuesta")]
    [ApiController]
    public class ProveedoresRespuestaController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IApplicationDbContext _context;

        public ProveedoresRespuestaController(IMediator mediator, IApplicationDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        /// <summary>
        /// El proveedor acepta todos los servicios asociados al token.
        /// GET /api/proveedores/respuesta/{token}/aceptar
        /// </summary>
        [HttpGet("{token}/aceptar")]
        public async Task<IActionResult> Aceptar(Guid token)
        {
            try
            {
                await _mediator.Send(new AceptarRespuestaProveedorCommand(token));

                var respuesta = await _context.RespuestaProveedor
                    .Include(r => r.Solicitud)
                    .Include(r => r.Proveedor)
                    .FirstOrDefaultAsync(r => r.Token == token);

                var html = RenderResultado(
                    headerColor: "#28a745",
                    titulo: "Servicios Aceptados",
                    icono: "✅",
                    mensaje: "Ha aceptado los servicios solicitados. El equipo de Capital Humano ha sido notificado.",
                    folio: respuesta?.Solicitud?.Folio ?? "—"
                );

                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                var html = RenderResultado(
                    headerColor: "#dc3545",
                    titulo: "Error",
                    icono: "❌",
                    mensaje: ex.Message,
                    folio: "—"
                );

                return Content(html, "text/html");
            }
        }

        /// <summary>
        /// Muestra formulario HTML para que el proveedor escriba el motivo del rechazo.
        /// GET /api/proveedores/respuesta/{token}/rechazar-form
        /// </summary>
        [HttpGet("{token}/rechazar-form")]
        public async Task<IActionResult> FormularioRechazo(Guid token)
        {
            try
            {
                var respuesta = await _context.RespuestaProveedor
                    .Include(r => r.Solicitud).ThenInclude(s => s.Obra)
                    .Include(r => r.Proveedor)
                    .FirstOrDefaultAsync(r => r.Token == token);

                if (respuesta == null)
                    return NotFound("Enlace no válido.");

                if (!respuesta.Vigente)
                {
                    var htmlYaRespondido = RenderResultado(
                        headerColor: "#6c757d",
                        titulo: "Enlace expirado",
                        icono: "⚠️",
                        mensaje: "Este enlace ya no es válido. Es posible que ya haya respondido anteriormente.",
                        folio: respuesta.Solicitud?.Folio ?? "—"
                    );
                    return Content(htmlYaRespondido, "text/html");
                }

                var template = EmailTemplateHelper.LoadTemplate("ProveedorRechazoForm.html");

                var urlRechazoPost = $"{Request.Scheme}://{Request.Host}/api/proveedores/respuesta/{token}/rechazar";

                var body = EmailTemplateHelper.Replace(template, new Dictionary<string, string>
                {
                    { "Proveedor", respuesta.Proveedor.Nombre },
                    { "Folio", respuesta.Solicitud.Folio },
                    { "Obra", respuesta.Solicitud.Obra?.Nombre ?? "—" },
                    { "FechaInicio", respuesta.Solicitud.FechaInicio.ToString("dd/MM/yyyy") },
                    { "FechaFin", respuesta.Solicitud.FechaFin.ToString("dd/MM/yyyy") },
                    { "UrlRechazoPost", urlRechazoPost }
                });

                return Content(body, "text/html");
            }
            catch (Exception ex)
            {
                var html = RenderResultado(
                    headerColor: "#dc3545",
                    titulo: "Error",
                    icono: "❌",
                    mensaje: ex.Message,
                    folio: "—"
                );

                return Content(html, "text/html");
            }
        }

        /// <summary>
        /// Procesa el rechazo del proveedor (form-urlencoded desde el HTML).
        /// POST /api/proveedores/respuesta/{token}/rechazar
        /// </summary>
        [HttpPost("{token}/rechazar")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> ConfirmarRechazo(Guid token, [FromForm] string comentarios)
        {
            try
            {
                await _mediator.Send(new RechazarRespuestaProveedorCommand(token, comentarios));

                var respuesta = await _context.RespuestaProveedor
                    .Include(r => r.Solicitud)
                    .FirstOrDefaultAsync(r => r.Token == token);

                var html = RenderResultado(
                    headerColor: "#dc3545",
                    titulo: "Servicios Rechazados",
                    icono: "🚫",
                    mensaje: "Ha rechazado los servicios solicitados. El equipo de Capital Humano ha sido notificado y se pondrá en contacto.",
                    folio: respuesta?.Solicitud?.Folio ?? "—"
                );

                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                var html = RenderResultado(
                    headerColor: "#dc3545",
                    titulo: "Error",
                    icono: "❌",
                    mensaje: ex.Message,
                    folio: "—"
                );

                return Content(html, "text/html");
            }
        }

        // ── Helpers ──────────────────────────────────────────────────

        private static string RenderResultado(string headerColor, string titulo, string icono, string mensaje, string folio)
        {
            var template = EmailTemplateHelper.LoadTemplate("AutorizacionResultado.html");

            return EmailTemplateHelper.Replace(template, new Dictionary<string, string>
            {
                { "HeaderColor", headerColor },
                { "Titulo", titulo },
                { "Icono", icono },
                { "Mensaje", mensaje },
                { "Folio", folio }
            });
        }
    }
}
