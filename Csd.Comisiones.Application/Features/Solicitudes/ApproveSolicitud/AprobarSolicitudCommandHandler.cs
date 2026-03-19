using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.ApproveSolicitud
{
    public class AprobarSolicitudCommandHandler : IRequestHandler<AprobarSolicitudCommand, bool>
    {
        private readonly ISolicitudRepository _solicitudRepository;

        public AprobarSolicitudCommandHandler(ISolicitudRepository solicitudRepository)
        {
            _solicitudRepository = solicitudRepository;
        }

        public async Task<bool> Handle(AprobarSolicitudCommand request, CancellationToken cancellationToken)
        {
            var solicitud = await _solicitudRepository.GetByIdAsync(request.SolicitudId);

            if(solicitud == null)
                throw new Exception("Solicitud no encontrada");

            solicitud.CambiarEstatus(EstatusSolicitudEnum.Terminada);

            await _solicitudRepository.UpdateAsync(solicitud);

            await _solicitudRepository.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
