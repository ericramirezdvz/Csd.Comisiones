using Csd.Comisiones.Application.Contracts.Infrastructure;
using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Application.Features.Proveedores.SendProveedores;
using Csd.Comisiones.Domain.Entities;
using Csd.Comisiones.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.ActualizarServicios
{
    public class ActualizarServiciosEmpleadoCommandHandler
        : IRequestHandler<ActualizarServiciosEmpleadoCommand, ActualizarServiciosResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public ActualizarServiciosEmpleadoCommandHandler(
            IApplicationDbContext context,
            IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<ActualizarServiciosResult> Handle(
            ActualizarServiciosEmpleadoCommand request,
            CancellationToken cancellationToken)
        {
            var solicitud = await _context.Solicitud
                .Include(s => s.Empleados)
                    .ThenInclude(e => e.Empleado)
                .Include(s => s.Empleados)
                    .ThenInclude(e => e.Hoteles)
                .Include(s => s.Empleados)
                    .ThenInclude(e => e.Comidas)
                .FirstOrDefaultAsync(s => s.SolicitudId == request.SolicitudId, cancellationToken)
                ?? throw new Exception("Solicitud no encontrada");

            var empleado = solicitud.Empleados
                .FirstOrDefault(e => e.EmpleadoId == request.EmpleadoId)
                ?? throw new Exception("Empleado no encontrado en la solicitud");

            // ── 0. Capturar estado ANTERIOR de servicios por proveedor ──
            var serviciosAnteriores = new List<(int proveedorId, string tipo, DateTime inicio, DateTime fin, decimal precio)>();

            foreach (var h in empleado.Hoteles
                .Where(h => h.ProveedorId.HasValue && h.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada))
            {
                var tipoHab = h.TipoHabitacionId == 2 ? "Hospedaje - Doble" : "Hospedaje - Sencilla";
                serviciosAnteriores.Add((h.ProveedorId!.Value, tipoHab, h.FechaInicio, h.FechaFin, h.PrecioUnitario));
            }
            foreach (var c in empleado.Comidas
                .Where(c => c.ProveedorId.HasValue && c.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada))
            {
                var tipoNombre = c.TipoComidaId switch { 1 => "Desayuno", 2 => "Comida", 3 => "Cena", _ => "Alimento" };
                serviciosAnteriores.Add((c.ProveedorId!.Value, tipoNombre, c.FechaInicio, c.FechaFin, c.PrecioUnitario));
            }

            // ── 1. Guardar info de servicios actuales para re-crear ──
            var lastHotel = empleado.Hoteles
                .Where(h => h.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada)
                .OrderByDescending(h => h.SolicitudHotelId)
                .FirstOrDefault();
            var hotelProveedorId = lastHotel?.ProveedorId;
            var hotelTipoHabitacionId = lastHotel?.TipoHabitacionId ?? 1;
            var hotelPrecio = lastHotel?.PrecioUnitario ?? 0;

            var preciosComida = new Dictionary<int, (decimal precio, int ubicacionId)>();
            foreach (var tipoId in new[] { 1, 2, 3 })
            {
                var last = empleado.Comidas
                    .Where(c => c.TipoComidaId == tipoId &&
                                c.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada)
                    .OrderByDescending(c => c.SolicitudComidaId)
                    .FirstOrDefault();
                preciosComida[tipoId] = (last?.PrecioUnitario ?? 0, last?.UbicacionAlimentoId ?? 1);
            }

            // ── 2. Cancelar todos los servicios activos ──
            foreach (var h in empleado.Hoteles
                .Where(h => h.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada))
            {
                h.Cancelar();
            }
            foreach (var c in empleado.Comidas
                .Where(c => c.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada))
            {
                c.Cancelar();
            }

            // ── 3. Re-crear servicios según nueva matriz de días ──
            var hospedajeDates = request.Dias
                .Where(d => d.Hospedaje)
                .Select(d => d.Fecha.Date)
                .OrderBy(d => d)
                .ToList();

            if (hotelProveedorId.HasValue && hospedajeDates.Count > 0)
            {
                foreach (var range in GroupContiguousDates(hospedajeDates))
                {
                    var hotel = new SolicitudHotel(
                        hotelProveedorId,
                        hotelTipoHabitacionId,
                        range.start,
                        range.end.AddDays(1),
                        hotelPrecio);
                    empleado.AgregarHotel(hotel);
                }
            }

            var mealConfig = new[]
            {
                new { TipoId = 1, ProveedorId = request.ProveedorDesayunoId,
                      DatesFilter = (Func<DiaServicioDto, bool>)(d => d.Desayuno) },
                new { TipoId = 2, ProveedorId = request.ProveedorComidaId,
                      DatesFilter = (Func<DiaServicioDto, bool>)(d => d.Comida) },
                new { TipoId = 3, ProveedorId = request.ProveedorCenaId,
                      DatesFilter = (Func<DiaServicioDto, bool>)(d => d.Cena) },
            };

            foreach (var meal in mealConfig)
            {
                if (!meal.ProveedorId.HasValue) continue;

                var dates = request.Dias
                    .Where(meal.DatesFilter)
                    .Select(d => d.Fecha.Date)
                    .OrderBy(d => d)
                    .ToList();

                if (dates.Count == 0) continue;

                var (precio, ubicacionId) = preciosComida[meal.TipoId];

                foreach (var range in GroupContiguousDates(dates))
                {
                    var comida = new SolicitudComida(
                        meal.TipoId,
                        meal.ProveedorId,
                        ubicacionId,
                        range.start,
                        range.end,
                        precio);
                    empleado.AgregarComida(comida);
                }
            }

            // ── 4. Capturar estado NUEVO de servicios por proveedor ──
            var serviciosNuevos = new List<(int proveedorId, string tipo, DateTime inicio, DateTime fin, decimal precio)>();

            foreach (var h in empleado.Hoteles
                .Where(h => h.ProveedorId.HasValue && h.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada))
            {
                var tipoHab = h.TipoHabitacionId == 2 ? "Hospedaje - Doble" : "Hospedaje - Sencilla";
                serviciosNuevos.Add((h.ProveedorId!.Value, tipoHab, h.FechaInicio, h.FechaFin, h.PrecioUnitario));
            }
            foreach (var c in empleado.Comidas
                .Where(c => c.ProveedorId.HasValue && c.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada))
            {
                var tipoNombre = c.TipoComidaId switch { 1 => "Desayuno", 2 => "Comida", 3 => "Cena", _ => "Alimento" };
                serviciosNuevos.Add((c.ProveedorId!.Value, tipoNombre, c.FechaInicio, c.FechaFin, c.PrecioUnitario));
            }

            // Todos los proveedores afectados (antes + ahora)
            var todosProveedores = serviciosAnteriores.Select(s => s.proveedorId)
                .Union(serviciosNuevos.Select(s => s.proveedorId))
                .ToHashSet();

            // ── 5. Notificar proveedores con diff de cambios ──
            var result = new ActualizarServiciosResult();

            if (todosProveedores.Count > 0)
            {
                // Invalidar todos los tokens vigentes de esta solicitud
                var tokensVigentes = await _context.RespuestaProveedor
                    .Where(r => r.SolicitudId == request.SolicitudId && r.Vigente)
                    .ToListAsync(cancellationToken);

                foreach (var token in tokensVigentes)
                    token.Invalidar();

                foreach (var proveedorId in todosProveedores)
                {
                    // Transicionar servicios nuevos a EnProceso
                    foreach (var h in empleado.Hoteles
                        .Where(h => h.EstatusDetalleId == (int)EstatusDetalleEnum.Borrador &&
                                    h.ProveedorId == proveedorId))
                    {
                        h.EnviarAProveedor();
                    }
                    foreach (var c in empleado.Comidas
                        .Where(c => c.EstatusDetalleId == (int)EstatusDetalleEnum.Borrador &&
                                    c.ProveedorId == proveedorId))
                    {
                        c.EnviarAProveedor();
                    }

                    // Crear token de respuesta
                    var proveedor = await _context.Proveedor
                        .FirstOrDefaultAsync(p => p.ProveedorId == proveedorId, cancellationToken);

                    if (proveedor == null) continue;

                    var respuesta = new RespuestaProveedor(request.SolicitudId, proveedorId);
                    _context.RespuestaProveedor.Add(respuesta);

                    result.ProveedoresAfectados.Add(proveedor.Nombre);

                    if (string.IsNullOrEmpty(proveedor.Correo)) continue;

                    // Compute diff for this provider
                    var antes = serviciosAnteriores
                        .Where(s => s.proveedorId == proveedorId)
                        .Select(s => (s.tipo, s.inicio, s.fin, s.precio))
                        .ToList();
                    var ahora = serviciosNuevos
                        .Where(s => s.proveedorId == proveedorId)
                        .Select(s => (s.tipo, s.inicio, s.fin, s.precio))
                        .ToList();

                    // Items in ahora but not in antes = agregados
                    var antesSet = new HashSet<(string, DateTime, DateTime, decimal)>(antes);
                    var ahoraSet = new HashSet<(string, DateTime, DateTime, decimal)>(ahora);

                    var agregados = ahora.Where(s => !antesSet.Contains(s))
                        .Select(s => new ProveedorDetalleDto
                        {
                            NombreEmpleado = empleado.Empleado.NombreCompleto,
                            TipoServicio = s.tipo,
                            FechaInicio = s.inicio,
                            FechaFin = s.fin,
                            PrecioUnitario = s.precio
                        }).ToList();

                    var eliminados = antes.Where(s => !ahoraSet.Contains(s))
                        .Select(s => new ProveedorDetalleDto
                        {
                            NombreEmpleado = empleado.Empleado.NombreCompleto,
                            TipoServicio = s.tipo,
                            FechaInicio = s.inicio,
                            FechaFin = s.fin,
                            PrecioUnitario = s.precio
                        }).ToList();

                    // Send modification email with diff
                    await _emailService.SendSolicitudProveedorModificadaAsync(
                        proveedor.Correo,
                        proveedor.Nombre,
                        solicitud.Folio,
                        empleado.Empleado.NombreCompleto,
                        agregados,
                        eliminados,
                        respuesta.Token);
                }

                result.ProveedoresNotificados = true;
            }

            await ((DbContext)_context).SaveChangesAsync(cancellationToken);

            return result;
        }

        private static IEnumerable<(DateTime start, DateTime end)> GroupContiguousDates(List<DateTime> dates)
        {
            if (dates.Count == 0) yield break;

            var start = dates[0];
            var prev = dates[0];

            for (int i = 1; i < dates.Count; i++)
            {
                if ((dates[i] - prev).TotalDays > 1)
                {
                    yield return (start, prev);
                    start = dates[i];
                }
                prev = dates[i];
            }

            yield return (start, prev);
        }
    }
}
