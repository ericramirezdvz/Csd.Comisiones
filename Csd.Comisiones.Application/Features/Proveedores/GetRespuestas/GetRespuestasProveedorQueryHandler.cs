using Csd.Comisiones.Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Csd.Comisiones.Application.Features.Proveedores.GetRespuestas
{
    public class GetRespuestasProveedorQueryHandler
        : IRequestHandler<GetRespuestasProveedorQuery, List<RespuestaProveedorDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetRespuestasProveedorQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RespuestaProveedorDto>> Handle(
            GetRespuestasProveedorQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.RespuestaProveedor
                .Where(r => r.SolicitudId == request.SolicitudId)
                .Include(r => r.Proveedor)
                .OrderByDescending(r => r.FechaEnvio)
                .Select(r => new RespuestaProveedorDto
                {
                    RespuestaProveedorId = r.RespuestaProveedorId,
                    ProveedorId = r.ProveedorId,
                    ProveedorNombre = r.Proveedor.Nombre,
                    FechaEnvio = r.FechaEnvio,
                    FechaRespuesta = r.FechaRespuesta,
                    Aceptado = r.Aceptado,
                    MotivoRechazo = r.MotivoRechazo,
                    Vigente = r.Vigente
                })
                .ToListAsync(cancellationToken);
        }
    }
}
