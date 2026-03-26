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

            solicitud.Rechazar(request.AutorizadorId, request.Comentarios);

            await _solicitudRepository.UpdateAsync(solicitud);

            await _solicitudRepository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
