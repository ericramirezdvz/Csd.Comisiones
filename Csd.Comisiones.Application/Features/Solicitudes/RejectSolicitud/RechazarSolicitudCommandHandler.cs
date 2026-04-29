using Csd.Comisiones.Application.Common.Models;
using Csd.Comisiones.Application.Contracts.Infrastructure;
using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.RejectSolicitud
{
    public class RechazarSolicitudCommandHandler
    : IRequestHandler<RechazarSolicitudCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly ISolicitudRepository _solicitudRepository;
        private readonly IEmailService _emailService;
        public RechazarSolicitudCommandHandler(IApplicationDbContext context, ISolicitudRepository solicitudRepository, IEmailService emailService)
        {
            _context = context;
            _solicitudRepository = solicitudRepository;
            _emailService = emailService;
        }

        public async Task<Unit> Handle(
            RechazarSolicitudCommand request,
            CancellationToken cancellationToken)
        {
            var solicitud = await _context.Solicitud
                .Include(x => x.Empleados)
                    .ThenInclude(e => e.Empleado)
                .Include(x => x.Empleados)
                    .ThenInclude(e => e.Hoteles)
                .Include(x => x.Empleados)
                    .ThenInclude(e => e.Comidas)
                .FirstOrDefaultAsync(x => x.SolicitudId == request.SolicitudId, cancellationToken);

            if (solicitud == null)
                throw new Exception("Solicitud no encontrada");

            if (solicitud.EstatusSolicitudId != (int)EstatusSolicitudEnum.EnAutorizaciónPorResponsableDeObra)
                throw new Exception("Solo se pueden rechazar solicitudes en proceso de autorización");

            var solicitante = await _context.Usuario
                .FirstOrDefaultAsync(u => u.UsuarioId == solicitud.SolicitanteId, cancellationToken);

            var correoSolicitante = solicitante?.Email ?? solicitante?.Username;
            if (solicitante == null || string.IsNullOrEmpty(correoSolicitante))
                throw new Exception("El solicitante no tiene correo configurado");

            solicitud.Rechazar(request.AutorizadorId, request.Comentarios);

            await _solicitudRepository.UpdateAsync(solicitud);

            await _solicitudRepository.SaveChangesAsync(cancellationToken);

            // Precios máximos de la ciudad para estimación
            var preciosMaximos = await _context.ProveedorServicio
                .Where(x => x.Proveedor.CiudadId == solicitud.CiudadId && x.Proveedor.Activo)
                .GroupBy(x => x.TipoServicio)
                .Select(g => new { TipoServicio = g.Key, PrecioMaximo = g.Max(x => x.Precio) })
                .ToDictionaryAsync(x => x.TipoServicio, x => x.PrecioMaximo, cancellationToken);

            decimal GetPrecioMax(TipoServicioEnum ts) =>
                preciosMaximos.TryGetValue(ts, out var p) ? p : 0;

            // Tipos de servicio desde metadata
            var comentarios = solicitud.Comentarios ?? "";
            var metaMatch = System.Text.RegularExpressions.Regex.Match(comentarios, @"##META:tiposServicio=([A-Z,]+)");
            var tiposServicio = metaMatch.Success
                ? metaMatch.Groups[1].Value.Split(',').ToHashSet()
                : new HashSet<string> { "HOSPEDAJE", "COMIDA" };
            if (!metaMatch.Success)
            {
                var legacy = System.Text.RegularExpressions.Regex.Match(comentarios, @"##META:tipoServicio=(HOSPEDAJE_COMIDA|HOSPEDAJE|COMIDA|PAGO)");
                if (legacy.Success)
                    tiposServicio = legacy.Groups[1].Value == "HOSPEDAJE_COMIDA"
                        ? new HashSet<string> { "HOSPEDAJE", "COMIDA" }
                        : new HashSet<string> { legacy.Groups[1].Value };
            }
            var incluyeHospedaje = tiposServicio.Contains("HOSPEDAJE");
            var incluyeComida = tiposServicio.Contains("COMIDA");

            var empleadosEmail = solicitud.Empleados
                .Select(e =>
                {
                    if (e.TipoAsignacion == TipoAsignacionEnum.Pago)
                    {
                        return new EmpleadoEmailDto
                        {
                            Nombre = e.EsExterno ? (e.NombreExterno ?? "Externo") : (e.Empleado?.NombreCompleto ?? "Sin nombre"),
                            FechaInicio = e.FechaInicio,
                            FechaFin = e.FechaFin,
                            RequiereHotel = false,
                            Desayuno = false,
                            Almuerzo = false,
                            Cena = false,
                            Total = e.MontoPago ?? 0,
                            EsPago = true
                        };
                    }

                    var hotelesActivos = e.Hoteles
                        .Where(h => h.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada);
                    var comidasActivas = e.Comidas
                        .Where(c => c.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada);

                    decimal totalHotel;
                    decimal totalComida;
                    bool requiereHotel;
                    bool desayuno, almuerzo, cena;

                    if (hotelesActivos.Any() || comidasActivas.Any())
                    {
                        totalHotel = hotelesActivos.Sum(h =>
                        {
                            var noches = (h.FechaFin - h.FechaInicio).Days;
                            return noches * h.PrecioUnitario;
                        });
                        totalComida = comidasActivas.Sum(c => c.PrecioUnitario);
                        requiereHotel = hotelesActivos.Any();
                        desayuno = comidasActivas.Any(c => c.TipoComidaId == (int)TipoComidaEnum.Desayuno);
                        almuerzo = comidasActivas.Any(c => c.TipoComidaId == (int)TipoComidaEnum.Almuerzo);
                        cena = comidasActivas.Any(c => c.TipoComidaId == (int)TipoComidaEnum.Cena);
                    }
                    else
                    {
                        var dias = (e.FechaFin - e.FechaInicio).Days;
                        var noches = Math.Max(dias - 1, 0);
                        totalHotel = incluyeHospedaje && noches > 0 ? noches * GetPrecioMax(TipoServicioEnum.HabitacionSencilla) : 0;
                        totalComida = incluyeComida && dias > 0 ? dias * 3 * GetPrecioMax(TipoServicioEnum.Alimento) : 0;
                        requiereHotel = incluyeHospedaje;
                        desayuno = incluyeComida;
                        almuerzo = incluyeComida;
                        cena = incluyeComida;
                    }

                    return new EmpleadoEmailDto
                    {
                        Nombre = e.EsExterno ? (e.NombreExterno ?? "Externo") : (e.Empleado?.NombreCompleto ?? "Sin nombre"),
                        FechaInicio = e.FechaInicio,
                        FechaFin = e.FechaFin,
                        RequiereHotel = requiereHotel,
                        Desayuno = desayuno,
                        Almuerzo = almuerzo,
                        Cena = cena,
                        Total = totalHotel + totalComida
                    };
                })
                .ToList();

            await _emailService.SendSolicitudRechazadaAsync(
                correoSolicitante,
                solicitud.Folio,
                solicitud.ObraId.ToString(),
                solicitud.FechaInicio,
                solicitud.FechaFin,
                empleadosEmail,
                request.Comentarios
            );

            return Unit.Value;
        }
    }
}
