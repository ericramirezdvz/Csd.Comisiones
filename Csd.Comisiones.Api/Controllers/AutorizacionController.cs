using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Application.Features.Solicitudes.ApproveSolicitud;
using Csd.Comisiones.Application.Features.Solicitudes.RejectSolicitud;
using Csd.Comisiones.Infrastructure.Email;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Csd.Comisiones.Api.Controllers
{
    /// <summary>
    /// Endpoints públicos para aprobar/rechazar solicitudes desde el correo electrónico.
    /// Los links del correo apuntan aquí (GET → HTML).
    /// </summary>
    [Route("api/solicitudes")]
    [ApiController]
    public class AutorizacionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IApplicationDbContext _context;

        public AutorizacionController(IMediator mediator, IApplicationDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        /// <summary>
        /// Aprueba la solicitud directamente desde el link del correo.
        /// GET /api/solicitudes/{id}/aprobar?autorizadorId=X
        /// </summary>
        [HttpGet("{id}/aprobar")]
        public async Task<IActionResult> AprobarDesdeCorreo(int id)
        {
            try
            {
                await _mediator.Send(new AprobarSolicitudCommand
                {
                    SolicitudId = id
                });

                var folio = await ObtenerFolio(id);

                var html = RenderResultado(
                    headerColor: "#28a745",
                    titulo: "Solicitud Aprobada",
                    icono: "✅",
                    mensaje: "La solicitud ha sido aprobada exitosamente.",
                    folio: folio
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
                    folio: $"Solicitud #{id}"
                );

                return Content(html, "text/html");
            }
        }

        /// <summary>
        /// Muestra el formulario de rechazo con textarea para el motivo.
        /// GET /api/solicitudes/{id}/rechazar-form?autorizadorId=X
        /// </summary>
        [HttpGet("{id}/rechazar-form")]
        public async Task<IActionResult> FormularioRechazo(int id, [FromQuery] int autorizadorId)
        {
            try
            {
                var solicitud = await _context.Solicitud.FindAsync(id);

                if (solicitud == null)
                    return NotFound("Solicitud no encontrada");

                var obra = await _context.Obra.FindAsync(solicitud.ObraId);

                var template = EmailTemplateHelper.LoadTemplate("RechazoForm.html");

                var urlRechazoPost = $"{Request.Scheme}://{Request.Host}/api/solicitudes/{id}/rechazar-confirmar?autorizadorId={autorizadorId}";

                var body = EmailTemplateHelper.Replace(template, new Dictionary<string, string>
                {
                    { "Folio", solicitud.Folio },
                    { "Obra", obra?.Nombre ?? solicitud.ObraId.ToString() },
                    { "FechaInicio", solicitud.FechaInicio.ToString("dd/MM/yyyy") },
                    { "FechaFin", solicitud.FechaFin.ToString("dd/MM/yyyy") },
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
                    folio: $"Solicitud #{id}"
                );

                return Content(html, "text/html");
            }
        }

        /// <summary>
        /// Recibe el POST del formulario de rechazo (form-urlencoded desde el HTML).
        /// POST /api/solicitudes/{id}/rechazar-confirmar?autorizadorId=X
        /// </summary>
        [HttpPost("{id}/rechazar-confirmar")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> ConfirmarRechazo(int id, [FromQuery] int autorizadorId, [FromForm] string comentarios)
        {
            try
            {
                await _mediator.Send(new RechazarSolicitudCommand
                {
                    SolicitudId = id,
                    AutorizadorId = autorizadorId,
                    Comentarios = comentarios
                });

                var folio = await ObtenerFolio(id);

                var html = RenderResultado(
                    headerColor: "#dc3545",
                    titulo: "Solicitud Rechazada",
                    icono: "🚫",
                    mensaje: "La solicitud ha sido rechazada. Se notificó al solicitante.",
                    folio: folio
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
                    folio: $"Solicitud #{id}"
                );

                return Content(html, "text/html");
            }
        }

        // ── Helpers ──────────────────────────────────────────────────

        private async Task<string> ObtenerFolio(int solicitudId)
        {
            var solicitud = await _context.Solicitud.FindAsync(solicitudId);
            return solicitud?.Folio ?? $"#{solicitudId}";
        }

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
