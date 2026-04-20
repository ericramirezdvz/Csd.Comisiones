using MediatR;

namespace Csd.Comisiones.Application.Features.Solicitudes.EnviarSolped
{
    public class EnviarSolpedCommand : IRequest<Unit>
    {
        public int SolicitudId { get; set; }
    }
}
