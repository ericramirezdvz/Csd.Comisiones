using Csd.Comisiones.Application.Contracts.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.CompleteSolicitud
{
    public class CompletarSolicitudCommandHandler : IRequestHandler<CompletarSolicitudCommand, Unit>
    {
        private readonly ISolicitudRepository _repository;

        public CompletarSolicitudCommandHandler(ISolicitudRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(CompletarSolicitudCommand request, CancellationToken cancellationToken)
        {
            var solicitud = await _repository.GetByIdAsync(request.SolicitudId);

            if (solicitud == null)
                throw new Exception("Solicitud no encontrada");

            solicitud.Terminar();

            await _repository.UpdateAsync(solicitud);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
