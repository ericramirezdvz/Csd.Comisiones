using Csd.Comisiones.Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Ciudades.GetCiudad
{
    public class GetCiudadQueryHandler : IRequestHandler<GetCiudadQuery, List<GetCiudadDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetCiudadQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<GetCiudadDto>> Handle(GetCiudadQuery request, CancellationToken cancellationToken)
        {
            return await _context.Ciudad
                .AsNoTracking()
                .Where(c => c.Activo)
                .Select(c => new GetCiudadDto
                {
                    CiudadId = c.CiudadId,
                    Nombre = c.Nombre
                }).ToListAsync(cancellationToken);
        }
    }
}
