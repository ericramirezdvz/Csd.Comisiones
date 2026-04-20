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

            // Agrupar servicios activos por proveedor para conciliación
            var serviciosHotel = solicitud.Empleados
                .SelectMany(e => e.Hoteles
                    .Where(h => h.ProveedorId != null && h.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada)
                    .Select(h => new { Proveedor = h.Proveedor, Empleado = e,
                        Tipo = h.TipoHabitacionId == 2 ? "Hospedaje - Doble" : "Hospedaje - Sencilla",
                        FechaInicio = h.FechaInicio, FechaFin = h.FechaFin,
                        Precio = h.PrecioUnitario }));

            var serviciosComida = solicitud.Empleados
                .SelectMany(e => e.Comidas
                    .Where(c => c.ProveedorId != null && c.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada)
                    .Select(c => new { Proveedor = c.Proveedor, Empleado = e,
                        Tipo = c.TipoComida?.Nombre ?? (c.TipoComidaId == 1 ? "Desayuno" : c.TipoComidaId == 2 ? "Comida" : "Cena"),
                        FechaInicio = c.FechaInicio, FechaFin = c.FechaFin,
                        Precio = c.PrecioUnitario }));

            var proveedores = serviciosHotel.Concat(serviciosComida)
                .GroupBy(x => x.Proveedor.ProveedorId).ToList();

            foreach (var grupo in proveedores)
            {
                var proveedor = grupo.First().Proveedor;

                var respuesta = new RespuestaProveedor(request.SolicitudId, proveedor.ProveedorId);
                _context.RespuestaProveedor.Add(respuesta);

                if (string.IsNullOrEmpty(proveedor.Correo)) continue;

                var detalles = grupo.Select(x => new ProveedorDetalleDto
                {
                    NombreEmpleado = x.Empleado.Empleado.NombreCompleto,
                    TipoServicio = x.Tipo,
                    FechaInicio = x.FechaInicio,
                    FechaFin = x.FechaFin,
                    PrecioUnitario = x.Precio
                }).ToList();

                await _emailService.SendSolicitudProveedorAsync(
                    proveedor.Correo, proveedor.Nombre, solicitud.Folio,
                    detalles, respuesta.Token, esConciliacion: true);
            }

            await ((DbContext)_context).SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
