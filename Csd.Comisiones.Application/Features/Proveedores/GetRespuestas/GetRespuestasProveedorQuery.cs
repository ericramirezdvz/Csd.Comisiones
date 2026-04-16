using MediatR;

namespace Csd.Comisiones.Application.Features.Proveedores.GetRespuestas
{
    public record GetRespuestasProveedorQuery(int SolicitudId) : IRequest<List<RespuestaProveedorDto>>;
}
