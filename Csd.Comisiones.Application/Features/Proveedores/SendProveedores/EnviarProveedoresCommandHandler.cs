using Csd.Comisiones.Application.Contracts.Infrastructure;
using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Domain.Entities;
using Csd.Comisiones.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Proveedores.SendProveedores
{
    public class EnviarProveedoresCommandHandler : IRequestHandler<EnviarProveedoresCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public EnviarProveedoresCommandHandler(
            IApplicationDbContext context,
            IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<Unit> Handle(EnviarProveedoresCommand request, CancellationToken cancellationToken)
        {
            var solicitud = await _context.Solicitud
                .Include(x => x.Empleados)
                    .ThenInclude(e => e.Empleado)
                .Include(x => x.Empleados)
                    .ThenInclude(e => e.Hoteles)
                        .ThenInclude(h => h.Proveedor)
                .Include(x => x.Empleados)
                    .ThenInclude(e => e.Comidas)
                        .ThenInclude(c => c.Proveedor)
                .FirstOrDefaultAsync(x => x.SolicitudId == request.SolicitudId, cancellationToken);

            if (solicitud == null)
                throw new Exception("Solicitud no encontrada");

            // Invalidar tokens anteriores para esta solicitud
            var tokensAnteriores = await _context.RespuestaProveedor
                .Where(r => r.SolicitudId == request.SolicitudId && r.Vigente)
                .ToListAsync(cancellationToken);

            foreach (var tokenAnterior in tokensAnteriores)
                tokenAnterior.Invalidar();

            // Recopilar servicios activos con proveedor asignado
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
                        EsHotel = true,
                        Hotel = (SolicitudHotel?)h,
                        Comida = (SolicitudComida?)null
                    }));

            var serviciosComida = solicitud.Empleados
                .SelectMany(e => e.Comidas
                    .Where(c => c.ProveedorId != null && c.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada)
                    .Select(c => new
                    {
                        ProveedorId = c.Proveedor.ProveedorId,
                        Proveedor = c.Proveedor,
                        EmpleadoNombre = e.EsExterno ? (e.NombreExterno ?? "Externo") : (e.Empleado?.NombreCompleto ?? "Sin nombre"),
                        TipoServicio = c.TipoComidaId == 1 ? "Desayuno" : c.TipoComidaId == 2 ? "Comida" : "Cena",
                        TipoHabitacionId = (int?)null,
                        FechaInicio = c.FechaInicio,
                        FechaFin = c.FechaFin,
                        Precio = c.PrecioUnitario,
                        EsHotel = false,
                        Hotel = (SolicitudHotel?)null,
                        Comida = (SolicitudComida?)c
                    }));

            var proveedores = serviciosHotel
                .Concat(serviciosComida)
                .GroupBy(x => x.ProveedorId)
                .ToList();

            // Subfolio: A, B, C... por cada proveedor
            var letraIdx = 0;

            foreach (var grupo in proveedores)
            {
                var proveedor = grupo.First().Proveedor;
                var subFolio = $"{solicitud.Folio}{(char)('A' + letraIdx)}";
                letraIdx++;

                // Transicionar servicios a EnProceso
                foreach (var servicio in grupo)
                {
                    servicio.Hotel?.EnviarAProveedor();
                    servicio.Comida?.EnviarAProveedor();
                }

                // Crear token de respuesta
                var respuesta = new RespuestaProveedor(request.SolicitudId, proveedor.ProveedorId);
                _context.RespuestaProveedor.Add(respuesta);

                if (string.IsNullOrEmpty(proveedor.Correo))
                    continue;

                // Construir detalles agrupados
                var detalles = BuildDetallesAgrupados(grupo.ToList());

                await _emailService.SendSolicitudProveedorAsync(
                    proveedor.Correo,
                    proveedor.Nombre,
                    solicitud.Folio,
                    subFolio,
                    detalles,
                    respuesta.Token
                );
            }

            await ((DbContext)_context).SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

        /// <summary>
        /// Agrupa servicios por tipo/fechas/precio. Para habitaciones dobles,
        /// agrupa empleados y calcula la cantidad de habitaciones (ceil(n/2)).
        /// Para sencillas y alimentos: 1 línea por empleado-servicio.
        /// </summary>
        private static List<ProveedorDetalleDto> BuildDetallesAgrupados<T>(List<T> servicios)
            where T : class
        {
            // Usamos dynamic para acceder a las propiedades del tipo anónimo
            var items = servicios.Cast<dynamic>().ToList();
            var result = new List<ProveedorDetalleDto>();

            // Separar hoteles dobles (se agrupan por habitación)
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
                    FechaInicio = g.Key.FI,
                    FechaFin = g.Key.FF,
                    PrecioUnitario = g.Key.P,
                    Empleados = empleados
                });
            }

            // Hoteles sencillos — 1 habitación por empleado
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
                    FechaInicio = g.Key.FI,
                    FechaFin = g.Key.FF,
                    PrecioUnitario = g.Key.P,
                    Empleados = empleados
                });
            }

            // Alimentos — agrupar por tipo + fechas + precio
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
                    FechaInicio = g.Key.FI,
                    FechaFin = g.Key.FF,
                    PrecioUnitario = g.Key.P,
                    Empleados = empleados
                });
            }

            return result;
        }
    }
}
