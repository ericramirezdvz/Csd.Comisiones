using Csd.Comisiones.Application.Contracts.Infrastructure;
using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Proveedores.RechazarRespuesta
{
    public class RechazarRespuestaProveedorCommandHandler
        : IRequestHandler<RechazarRespuestaProveedorCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public RechazarRespuestaProveedorCommandHandler(
            IApplicationDbContext context,
            IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<Unit> Handle(RechazarRespuestaProveedorCommand request, CancellationToken cancellationToken)
        {
            var respuesta = await _context.RespuestaProveedor
                .Include(r => r.Proveedor)
                .FirstOrDefaultAsync(r => r.Token == request.Token, cancellationToken);

            if (respuesta == null)
                throw new Exception("Enlace no válido.");

            // Marca la respuesta como rechazada (valida internamente que esté vigente)
            respuesta.Rechazar(request.Comentarios);

            // Cargar solicitud con servicios de este proveedor
            var solicitud = await _context.Solicitud
                .Include(s => s.Empleados).ThenInclude(e => e.Hoteles)
                .Include(s => s.Empleados).ThenInclude(e => e.Comidas)
                .FirstOrDefaultAsync(s => s.SolicitudId == respuesta.SolicitudId, cancellationToken)
                ?? throw new Exception("Solicitud no encontrada.");

            // Rechazar todos los servicios de este proveedor que estén EnProceso
            foreach (var empleado in solicitud.Empleados)
            {
                foreach (var hotel in empleado.Hoteles
                    .Where(h => h.ProveedorId == respuesta.ProveedorId
                             && h.EstatusDetalleId == (int)EstatusDetalleEnum.EnProceso))
                {
                    hotel.RechazarPorProveedor();
                }

                foreach (var comida in empleado.Comidas
                    .Where(c => c.ProveedorId == respuesta.ProveedorId
                             && c.EstatusDetalleId == (int)EstatusDetalleEnum.EnProceso))
                {
                    comida.RechazarPorProveedor();
                }
            }

            await ((DbContext)_context).SaveChangesAsync(cancellationToken);

            // Notificar a CH del rechazo
            await _emailService.SendProveedorRechazoNotificacionAsync(
                solicitud.Folio,
                respuesta.Proveedor.Nombre,
                request.Comentarios
            );

            return Unit.Value;
        }
    }
}
