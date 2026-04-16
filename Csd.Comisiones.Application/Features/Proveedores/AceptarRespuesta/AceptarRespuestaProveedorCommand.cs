using MediatR;
using System;

namespace Csd.Comisiones.Application.Features.Proveedores.AceptarRespuesta
{
    public record AceptarRespuestaProveedorCommand(Guid Token) : IRequest<Unit>;
}
