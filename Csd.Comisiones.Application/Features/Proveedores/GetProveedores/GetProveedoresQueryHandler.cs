using Csd.Comisiones.Application.Common.Extensions;
using Csd.Comisiones.Application.Common.Models;
using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Csd.Comisiones.Application.Features.Proveedores.GetProveedores
{
    public class GetProveedoresQueryHandler : IRequestHandler<GetProveedoresQuery, PagedResult<ProveedorDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetProveedoresQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<ProveedorDto>> Handle(
            GetProveedoresQuery request,
            CancellationToken cancellationToken)
        {

            var query = _context.Proveedor
                .AsNoTracking()
                .Where(x => x.Activo);

            if (request.CiudadId.HasValue)
            {
                query = query.Where(p => p.CiudadId == request.CiudadId.Value);
            }

            if (request.TipoProveedorId.HasValue)
            {
                query = query.Where(p => p.TipoProveedor == (TipoProveedorEnum)request.TipoProveedorId.Value);
            }

            query = query.OrderBy(p => p.Nombre);

            return await query
                .Select(p => new ProveedorDto
                {
                    ProveedorId = p.ProveedorId,
                    Nombre = p.Nombre,
                    CiudadId = p.CiudadId,
                    ProporcionaHospedaje = p.ProporcionaHospedaje,
                    ProporcionaAlimentos = p.ProporcionaAlimentos,
                    Servicios = p.Servicios.Select(s => new ProveedorServicioDto
                    {
                        TipoServicioId = (int)s.TipoServicio,
                        TipoServicioNombre = s.TipoServicio.ToString(),
                        Precio = s.Precio
                    }).ToList()
                })
                .ToPagedResultAsync(
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken
                );
        }
    }
}

