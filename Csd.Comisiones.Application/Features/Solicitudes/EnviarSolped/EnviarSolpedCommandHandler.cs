using Csd.Comisiones.Application.Contracts.Infrastructure;
using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Csd.Comisiones.Application.Features.Solicitudes.EnviarSolped
{
    public class EnviarSolpedCommandHandler : IRequestHandler<EnviarSolpedCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ISolpedExcelService _excelService;

        public EnviarSolpedCommandHandler(
            IApplicationDbContext context,
            IEmailService emailService,
            ISolpedExcelService excelService)
        {
            _context = context;
            _emailService = emailService;
            _excelService = excelService;
        }

        public async Task<Unit> Handle(EnviarSolpedCommand request, CancellationToken cancellationToken)
        {
            var solicitud = await _context.Solicitud
                .Include(s => s.Obra)
                .Include(s => s.Area)
                .Include(s => s.Ciudad)
                .Include(s => s.MotivoSolicitud)
                .Include(s => s.Empleados)
                    .ThenInclude(e => e.Empleado)
                .Include(s => s.Empleados)
                    .ThenInclude(e => e.Hoteles)
                        .ThenInclude(h => h.Proveedor)
                .Include(s => s.Empleados)
                    .ThenInclude(e => e.Comidas)
                        .ThenInclude(c => c.TipoComida)
                .Include(s => s.Empleados)
                    .ThenInclude(e => e.Comidas)
                        .ThenInclude(c => c.Proveedor)
                .FirstOrDefaultAsync(s => s.SolicitudId == request.SolicitudId, cancellationToken)
                ?? throw new Exception($"No se encontró la solicitud {request.SolicitudId}");

            if (solicitud.EstatusSolicitudId != (int)EstatusSolicitudEnum.Terminada)
                throw new InvalidOperationException("Solo se pueden enviar Solpeds de comisiones terminadas.");

            var fechaInicio = solicitud.FechaInicio.Date;
            var fechaFin = solicitud.FechaFin.Date;

            // Build day list for the commission range
            var dias = new List<DateTime>();
            for (var d = fechaInicio; d <= fechaFin; d = d.AddDays(1))
                dias.Add(d);

            // ═══ TABLA DE ALIMENTACIÓN ═══
            var tablaAlimentacion = BuildTablaAlimentacion(solicitud, dias);

            // ═══ TABLA DE HOSPEDAJE ═══
            var tablaHospedaje = BuildTablaHospedaje(solicitud, dias);

            var periodo = $"{fechaInicio:dd/MM/yyyy} — {fechaFin:dd/MM/yyyy}";

            // Generate Excel attachment
            var excelBytes = _excelService.GenerarExcelSolped(solicitud);

            await _emailService.SendSolpedAsync(
                solicitud.Folio,
                solicitud.Obra?.Nombre ?? "",
                solicitud.Area?.Nombre ?? "",
                periodo,
                tablaAlimentacion,
                tablaHospedaje,
                excelBytes);

            // ═══ GENERADOR POR PROVEEDOR ═══
            var generadores = _excelService.GenerarExcelPorProveedor(solicitud);
            foreach (var (_, (provNombre, provCorreo, excelProveedor)) in generadores)
            {
                if (string.IsNullOrWhiteSpace(provCorreo))
                    continue;

                await _emailService.SendGeneradorProveedorAsync(
                    provCorreo,
                    provNombre,
                    solicitud.Folio,
                    excelProveedor);
            }

            return Unit.Value;
        }

        private static string BuildTablaAlimentacion(
            Domain.Entities.Solicitud solicitud,
            List<DateTime> dias)
        {
            var empleados = solicitud.Empleados
                .Where(e => e.TipoAsignacion != TipoAsignacionEnum.Pago)
                .ToList();

            if (!empleados.Any())
                return "<p style='color:#888;'>No hay empleados con servicio de alimentación.</p>";

            var sb = new System.Text.StringBuilder();
            sb.Append(@"<table width='100%' border='0' cellpadding='4' cellspacing='0' 
                style='border-collapse:collapse; font-size:11px;'>");

            // Header row 1 — day numbers
            sb.Append("<thead>");
            sb.Append("<tr style='background-color:#06205c; color:white; text-align:center;'>");
            sb.Append("<th rowspan='2' style='padding:6px; border:1px solid #ccc; min-width:150px;'>Nombre</th>");
            sb.Append("<th rowspan='2' style='padding:6px; border:1px solid #ccc;'>No. Ficha</th>");

            foreach (var dia in dias)
            {
                sb.Append($"<th colspan='3' style='padding:4px; border:1px solid #ccc;'>{dia:dd}</th>");
            }

            sb.Append("<th rowspan='2' style='padding:6px; border:1px solid #ccc;'>Total</th>");
            sb.Append("</tr>");

            // Header row 2 — D/C/C sub-columns
            sb.Append("<tr style='background-color:#01a29c; color:white; text-align:center; font-size:10px;'>");
            foreach (var _ in dias)
            {
                sb.Append("<th style='padding:2px; border:1px solid #ccc;'>D</th>");
                sb.Append("<th style='padding:2px; border:1px solid #ccc;'>C</th>");
                sb.Append("<th style='padding:2px; border:1px solid #ccc;'>Ce</th>");
            }
            sb.Append("</tr>");
            sb.Append("</thead><tbody>");

            // Data rows
            foreach (var emp in empleados)
            {
                sb.Append("<tr style='border-bottom:1px solid #eee; text-align:center;'>");
                sb.Append($"<td style='padding:4px; border:1px solid #eee; text-align:left; white-space:nowrap;'>{(emp.EsExterno ? (emp.NombreExterno ?? "Externo") : (emp.Empleado?.NombreCompleto ?? "Sin nombre"))}</td>");
                sb.Append($"<td style='padding:4px; border:1px solid #eee;'>{(emp.EsExterno ? "" : (emp.Empleado?.NumeroEmpleado ?? ""))}</td>");

                int totalServicios = 0;

                foreach (var dia in dias)
                {
                    // Desayuno (TipoComidaId = 1)
                    bool desayuno = emp.Comidas.Any(c =>
                        c.TipoComidaId == 1 &&
                        c.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada &&
                        dia.Date >= c.FechaInicio.Date && dia.Date <= c.FechaFin.Date);

                    // Comida (TipoComidaId = 2)
                    bool comida = emp.Comidas.Any(c =>
                        c.TipoComidaId == 2 &&
                        c.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada &&
                        dia.Date >= c.FechaInicio.Date && dia.Date <= c.FechaFin.Date);

                    // Cena (TipoComidaId = 3)
                    bool cena = emp.Comidas.Any(c =>
                        c.TipoComidaId == 3 &&
                        c.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada &&
                        dia.Date >= c.FechaInicio.Date && dia.Date <= c.FechaFin.Date);

                    sb.Append($"<td style='padding:2px; border:1px solid #eee;'>{(desayuno ? "X" : "")}</td>");
                    sb.Append($"<td style='padding:2px; border:1px solid #eee;'>{(comida ? "X" : "")}</td>");
                    sb.Append($"<td style='padding:2px; border:1px solid #eee;'>{(cena ? "X" : "")}</td>");

                    if (desayuno) totalServicios++;
                    if (comida) totalServicios++;
                    if (cena) totalServicios++;
                }

                sb.Append($"<td style='padding:4px; border:1px solid #eee; font-weight:bold;'>{totalServicios}</td>");
                sb.Append("</tr>");
            }

            sb.Append("</tbody></table>");
            return sb.ToString();
        }

        private static string BuildTablaHospedaje(
            Domain.Entities.Solicitud solicitud,
            List<DateTime> dias)
        {
            var empleados = solicitud.Empleados
                .Where(e => e.TipoAsignacion != TipoAsignacionEnum.Pago && e.Hoteles.Any())
                .ToList();

            if (!empleados.Any())
                return "<p style='color:#888;'>No hay empleados con servicio de hospedaje.</p>";

            var sb = new System.Text.StringBuilder();
            sb.Append(@"<table width='100%' border='0' cellpadding='4' cellspacing='0' 
                style='border-collapse:collapse; font-size:11px;'>");

            // Header
            sb.Append("<thead>");
            sb.Append("<tr style='background-color:#06205c; color:white; text-align:center;'>");
            sb.Append("<th style='padding:6px; border:1px solid #ccc; min-width:150px;'>Nombre</th>");
            sb.Append("<th style='padding:6px; border:1px solid #ccc;'>No. Ficha</th>");
            sb.Append("<th style='padding:6px; border:1px solid #ccc;'>Tipo Hab.</th>");

            foreach (var dia in dias)
            {
                sb.Append($"<th style='padding:4px; border:1px solid #ccc;'>{dia:dd}</th>");
            }

            sb.Append("<th style='padding:6px; border:1px solid #ccc;'>Total D&iacute;as</th>");
            sb.Append("</tr>");
            sb.Append("</thead><tbody>");

            int totalSencillas = 0;
            int totalDobles = 0;

            // Data rows
            foreach (var emp in empleados)
            {
                // Take room type from the first active hotel record
                var hotelActivo = emp.Hoteles
                    .FirstOrDefault(h => h.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada);
                var tipoHab = hotelActivo?.TipoHabitacionId == 2 ? "Doble" : "Sencilla";
                bool esDoble = hotelActivo?.TipoHabitacionId == 2;

                sb.Append("<tr style='border-bottom:1px solid #eee; text-align:center;'>");
                sb.Append($"<td style='padding:4px; border:1px solid #eee; text-align:left; white-space:nowrap;'>{(emp.EsExterno ? (emp.NombreExterno ?? "Externo") : (emp.Empleado?.NombreCompleto ?? "Sin nombre"))}</td>");
                sb.Append($"<td style='padding:4px; border:1px solid #eee;'>{(emp.EsExterno ? "" : (emp.Empleado?.NumeroEmpleado ?? ""))}</td>");
                sb.Append($"<td style='padding:4px; border:1px solid #eee;'>{tipoHab}</td>");

                int totalDias = 0;

                foreach (var dia in dias)
                {
                    bool pernocta = emp.Hoteles.Any(h =>
                        h.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada &&
                        dia.Date >= h.FechaInicio.Date && dia.Date < h.FechaFin.Date);

                    sb.Append($"<td style='padding:2px; border:1px solid #eee;'>{(pernocta ? "X" : "")}</td>");
                    if (pernocta) totalDias++;
                }

                if (esDoble)
                    totalDobles += totalDias;
                else
                    totalSencillas += totalDias;

                sb.Append($"<td style='padding:4px; border:1px solid #eee; font-weight:bold;'>{totalDias}</td>");
                sb.Append("</tr>");
            }

            sb.Append("</tbody>");

            // Summary footer — proper room-night count (double = 2 employees per room)
            var habitacionesSencillas = totalSencillas;
            var habitacionesDobles = (int)Math.Ceiling(totalDobles / 2.0);
            var totalHabitaciones = habitacionesSencillas + habitacionesDobles;

            sb.Append("<tfoot>");
            sb.Append("<tr style='background-color:#f0f0f0; text-align:center; font-weight:bold;'>");
            sb.Append($"<td colspan='3' style='padding:6px; border:1px solid #ccc; text-align:right;'>Total habitaciones-noche:</td>");
            sb.Append($"<td colspan='{dias.Count}' style='padding:6px; border:1px solid #ccc; text-align:left;'>");
            if (habitacionesSencillas > 0)
                sb.Append($"Sencillas: {habitacionesSencillas}");
            if (habitacionesSencillas > 0 && habitacionesDobles > 0)
                sb.Append(" &nbsp;|&nbsp; ");
            if (habitacionesDobles > 0)
                sb.Append($"Dobles: {habitacionesDobles}");
            sb.Append("</td>");
            sb.Append($"<td style='padding:6px; border:1px solid #ccc;'>{totalHabitaciones}</td>");
            sb.Append("</tr>");
            sb.Append("</tfoot>");

            sb.Append("</table>");
            return sb.ToString();
        }
    }
}
