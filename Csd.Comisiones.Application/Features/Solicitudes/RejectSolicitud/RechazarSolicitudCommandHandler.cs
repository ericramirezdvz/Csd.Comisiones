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
        public RechazarSolicitudCommandHandler(IApplicationDbContext context, ISolicitudRepository solicitudRepository)
        {
            _context = context;
            _solicitudRepository = solicitudRepository;
        }

        public async Task<Unit> Handle(
            RechazarSolicitudCommand request,
            CancellationToken cancellationToken)
        {
            var solicitud = await _context.Solicitud
                .Include(x => x.Autorizaciones)
                .FirstOrDefaultAsync(x => x.SolicitudId == request.SolicitudId, cancellationToken);

            if (solicitud == null)
                throw new Exception("Solicitud no encontrada");

            // Validar estado
            if (solicitud.EstatusSolicitudId != (int)EstatusSolicitudEnum.EnAutorizaciónPorResponsableDeObra)
                throw new Exception("La solicitud no está en proceso de autorización");

            // Buscar autorización del usuario
            var autorizacion = solicitud.Autorizaciones
                .FirstOrDefault(x => x.AutorizadorId == request.AutorizadorId);

            if (autorizacion == null)
                throw new Exception("No tienes autorización para esta solicitud");

            if (autorizacion.EstatusAutorizacionId != (int)EstatusAutorizacionEnum.Pendiente)
                throw new Exception("Esta autorización ya fue atendida");

            // Rechazar
            autorizacion.Rechazar(request.Comentarios);

            // Cancelar las demás pendientes
            foreach (var otra in solicitud.Autorizaciones
                         .Where(x => x.SolicitudAutorizacionId != autorizacion.SolicitudAutorizacionId
                                  && x.EstatusAutorizacionId == (int)EstatusAutorizacionEnum.Pendiente))
            {
                // opción simple: no hacer nada (quedan pendientes pero irrelevantes)
                // opción pro: marcar como canceladas (requiere nuevo estatus)
            }

            // Cambiar estatus de solicitud
            solicitud.CambiarEstatus(EstatusSolicitudEnum.Rechazada);

            await _solicitudRepository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
