using Csd.Comisiones.Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Areas.GetAreas
{
    public class GetAreasQueryHandler : IRequestHandler<GetAreasQuery, List<AreaDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAreasQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AreaDto>> Handle(GetAreasQuery request, CancellationToken cancellationToken)
        {
            return await _context.Area
                .Where(a => a.Activo)
                .OrderBy(a => a.Nombre)
                .Select(a => new AreaDto
                {
                    Id = a.AreaId,
                    Nombre = a.Nombre,
                    Descripcion = a.Descripcion
                })
                .ToListAsync(cancellationToken);
        }
    }
}
