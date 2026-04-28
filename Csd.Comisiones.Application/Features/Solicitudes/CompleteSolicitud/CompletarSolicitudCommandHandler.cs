using Csd.Comisiones.Application.Contracts.Infrastructure;
using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Application.Features.Proveedores.SendProveedores;
using Csd.Comisiones.Domain.Entities;
using Csd.Comisiones.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Csd.Comisiones.Application.Features.Solicitudes.CompleteSolicitud
{
    public class CompletarSolicitudCommandHandler : IRequestHandler<CompletarSolicitudCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public CompletarSolicitudCommandHandler(IApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<Unit> Handle(CompletarSolicitudCommand request, CancellationToken cancellationToken)
        {
            var solicitud = await _context.Solicitud
                .Include(x => x.Empleados).ThenInclude(e => e.Empleado)
                .Include(x => x.Empleados).ThenInclude(e => e.Hoteles).ThenInclude(h => h.Proveedor)
                .Include(x => x.Empleados).ThenInclude(e => e.Comidas).ThenInclude(c => c.Proveedor)
                .Include(x => x.Empleados).ThenInclude(e => e.Comidas).ThenInclude(c => c.TipoComida)
                .FirstOrDefaultAsync(x => x.SolicitudId == request.SolicitudId, cancellationToken);

            if (solicitud == null)
                throw new Exception("Solicitud no encontrada");

            solicitud.Terminar();

            // Invalidar tokens anteriores
            var tokensAnteriores = await _context.RespuestaProveedor
                .Where(r => r.SolicitudId == request.SolicitudId && r.Vigente)
                .ToListAsync(cancellationToken);

            foreach (var token in tokensAnteriores)
                token.Invalidar();

            // Recopilar servicios activos
            var serviciosHotel = solicitud.Empleados
                .SelectMany(e => e.Hoteles
                    .Where(h => h.ProveedorId != null && h.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada)
                    .Select(h => new
                    {
                        ProveedorId = h.Proveedor.ProveedorId,
                        Proveedor = h.Proveedor,
                        EmpleadoNombre = e.EsExterno ? (e.NombreExterno ?? "Externo") : (e.Empleado?.NombreCompleto ?? "Sin nombre"),
                        TipoServicio = h.TipoHabitacionId == 2 ? "Hospedaje - Doble" : "Hospedaje - Sencilla",
                        TipoHabitacionId = (int?)h.TipoHabitacionId,
                        FechaInicio = h.FechaInicio,
                        FechaFin = h.FechaFin,
                        Precio = h.PrecioUnitario,
                        EsHotel = true
                    }));

            var serviciosComida = solicitud.Empleados
                .SelectMany(e => e.Comidas
                    .Where(c => c.ProveedorId != null && c.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada)
                    .Select(c => new
                    {
                        ProveedorId = c.Proveedor.ProveedorId,
                        Proveedor = c.Proveedor,
                        EmpleadoNombre = e.EsExterno ? (e.NombreExterno ?? "Externo") : (e.Empleado?.NombreCompleto ?? "Sin nombre"),
                        TipoServicio = c.TipoComida?.Nombre ?? (c.TipoComidaId == 1 ? "Desayuno" : c.TipoComidaId == 2 ? "Comida" : "Cena"),
                        TipoHabitacionId = (int?)null,
                        FechaInicio = c.FechaInicio,
                        FechaFin = c.FechaFin,
                        Precio = c.PrecioUnitario,
                        EsHotel = false
                    }));

            var proveedores = serviciosHotel.Concat(serviciosComida)
                .GroupBy(x => x.ProveedorId).ToList();

            var letraIdx = 0;

            foreach (var grupo in proveedores)
            {
                var proveedor = grupo.First().Proveedor;
                var subFolio = $"{solicitud.Folio}{(char)('A' + letraIdx)}";
                letraIdx++;

                var respuesta = new RespuestaProveedor(request.SolicitudId, proveedor.ProveedorId);
                _context.RespuestaProveedor.Add(respuesta);

                if (string.IsNullOrEmpty(proveedor.Correo)) continue;

                var detalles = BuildDetallesAgrupados(grupo.ToList());

                await _emailService.SendSolicitudProveedorAsync(
                    proveedor.Correo, proveedor.Nombre, solicitud.Folio,
                    subFolio, detalles, respuesta.Token, esConciliacion: true);
            }

            await ((DbContext)_context).SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        /// <summary>
        /// Agrupa servicios por tipo/fechas/precio. Habitaciones dobles se agrupan
        /// calculando ceil(empleados/2). Alimentos y sencillas se agrupan por cantidad.
        /// </summary>
        private static List<ProveedorDetalleDto> BuildDetallesAgrupados<T>(List<T> servicios)
            where T : class
        {
            var items = servicios.Cast<dynamic>().ToList();
            var result = new List<ProveedorDetalleDto>();

            // Hoteles dobles → agrupar por fechas/precio, calcular habitaciones
            var hotelesDobles = items
                .Where(x => (bool)x.EsHotel && x.TipoHabitacionId == 2)
                .GroupBy(x => new { FI = (DateTime)x.FechaInicio, FF = (DateTime)x.FechaFin, P = (decimal)x.Precio })
                .ToList();

            foreach (var g in hotelesDobles)
            {
                var empleados = g.Select(x => (string)x.EmpleadoNombre).ToList();
                var habitaciones = (int)Math.Ceiling(empleados.Count / 2.0);
                var noches = Math.Max((g.Key.FF - g.Key.FI).Days, 1);
                result.Add(new ProveedorDetalleDto
                {
                    TipoServicio = $"Hospedaje - Doble ({habitaciones} hab.)",
                    Cantidad = habitaciones,
                    Dias = noches,
                    FechaInicio = g.Key.FI, FechaFin = g.Key.FF,
                    PrecioUnitario = g.Key.P,
                    Empleados = empleados
                });
            }

            // Hoteles sencillos → 1 hab. por empleado
            var hotelesSencillos = items
                .Where(x => (bool)x.EsHotel && x.TipoHabitacionId != 2)
                .GroupBy(x => new { FI = (DateTime)x.FechaInicio, FF = (DateTime)x.FechaFin, P = (decimal)x.Precio })
                .ToList();

            foreach (var g in hotelesSencillos)
            {
                var empleados = g.Select(x => (string)x.EmpleadoNombre).ToList();
                var noches = Math.Max((g.Key.FF - g.Key.FI).Days, 1);
                result.Add(new ProveedorDetalleDto
                {
                    TipoServicio = "Hospedaje - Sencilla",
                    Cantidad = empleados.Count,
                    Dias = noches,
                    FechaInicio = g.Key.FI, FechaFin = g.Key.FF,
                    PrecioUnitario = g.Key.P,
                    Empleados = empleados
                });
            }

            // Alimentos → agrupar por tipo + fechas + precio
            var alimentos = items
                .Where(x => !(bool)x.EsHotel)
                .GroupBy(x => new { Tipo = (string)x.TipoServicio, FI = (DateTime)x.FechaInicio, FF = (DateTime)x.FechaFin, P = (decimal)x.Precio })
                .ToList();

            foreach (var g in alimentos)
            {
                var empleados = g.Select(x => (string)x.EmpleadoNombre).ToList();
                // Días calendario inclusivos (considerar hora de inicio/fin)
                var dias = Math.Max((g.Key.FF.Date - g.Key.FI.Date).Days + 1, 1);
                result.Add(new ProveedorDetalleDto
                {
                    TipoServicio = g.Key.Tipo,
                    Cantidad = empleados.Count,
                    Dias = dias,
                    FechaInicio = g.Key.FI, FechaFin = g.Key.FF,
                    PrecioUnitario = g.Key.P,
                    Empleados = empleados
                });
            }

            return result;
        }
    }
}
