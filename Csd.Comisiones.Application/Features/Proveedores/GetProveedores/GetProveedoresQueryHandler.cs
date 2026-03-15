using Csd.Comisiones.Application.Common.Extensions;
using Csd.Comisiones.Application.Common.Models;
using Csd.Comisiones.Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
                .Where(x => x.Activo)
                .Select(p => new ProveedorDto
                {
                    ProveedorId = p.ProveedorId,
                    Nombre = p.Nombre
                });

            var totalCount = await query.CountAsync(cancellationToken);

            return await query.ToPagedResultAsync(
                request.PageNumber,
                request.PageSize,
                cancellationToken
                );
        }
    }
}

