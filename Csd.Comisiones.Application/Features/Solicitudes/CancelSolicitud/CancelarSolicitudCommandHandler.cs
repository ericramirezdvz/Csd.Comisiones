using Csd.Comisiones.Application.Contracts.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.CancelSolicitud
{
    public class CancelarSolicitudCommandHandler
    : IRequestHandler<CancelarSolicitudCommand, Unit>
    {
        private readonly ISolicitudRepository _repository;

        public CancelarSolicitudCommandHandler(ISolicitudRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(
            CancelarSolicitudCommand request,
            CancellationToken cancellationToken)
        {
            var solicitud = await _repository.GetByIdAsync(request.SolicitudId);

            if (solicitud == null)
                throw new Exception("Solicitud no encontrada");

            solicitud.Cancelar(request.Comentarios);

            await _repository.UpdateAsync(solicitud);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
