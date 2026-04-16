using MediatR;
using System;

namespace Csd.Comisiones.Application.Features.Proveedores.RechazarRespuesta
{
    public record RechazarRespuestaProveedorCommand(Guid Token, string Comentarios) : IRequest<Unit>;
}
