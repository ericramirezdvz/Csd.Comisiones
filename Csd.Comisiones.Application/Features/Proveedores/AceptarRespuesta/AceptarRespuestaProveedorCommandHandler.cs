using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Proveedores.AceptarRespuesta
{
    public class AceptarRespuestaProveedorCommandHandler
        : IRequestHandler<AceptarRespuestaProveedorCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public AceptarRespuestaProveedorCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(AceptarRespuestaProveedorCommand request, CancellationToken cancellationToken)
        {
            var respuesta = await _context.RespuestaProveedor
                .FirstOrDefaultAsync(r => r.Token == request.Token, cancellationToken);

            if (respuesta == null)
                throw new Exception("Enlace no válido.");

            // Marca la respuesta como aceptada (valida internamente que esté vigente)
            respuesta.Aceptar();

            // Cargar solicitud con servicios de este proveedor
            var solicitud = await _context.Solicitud
                .Include(s => s.Empleados).ThenInclude(e => e.Hoteles)
                .Include(s => s.Empleados).ThenInclude(e => e.Comidas)
                .FirstOrDefaultAsync(s => s.SolicitudId == respuesta.SolicitudId, cancellationToken)
                ?? throw new Exception("Solicitud no encontrada.");

            // Aceptar todos los servicios de este proveedor que estén EnProceso
            foreach (var empleado in solicitud.Empleados)
            {
                foreach (var hotel in empleado.Hoteles
                    .Where(h => h.ProveedorId == respuesta.ProveedorId
                             && h.EstatusDetalleId == (int)EstatusDetalleEnum.EnProceso))
                {
                    hotel.AceptarPorProveedor();
                }

                foreach (var comida in empleado.Comidas
                    .Where(c => c.ProveedorId == respuesta.ProveedorId
                             && c.EstatusDetalleId == (int)EstatusDetalleEnum.EnProceso))
                {
                    comida.AceptarPorProveedor();
                }
            }

            await ((DbContext)_context).SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
