using Csd.Comisiones.Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.TiposComida.GetTipoComida
{
    public class GetTipoComidaQueryHandler : IRequestHandler<GetTipoComidaQuery, List<GetTipoComidaDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetTipoComidaQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<GetTipoComidaDto>> Handle(GetTipoComidaQuery request, CancellationToken cancellationToken)
        {
            return await _context.TipoComida
                .AsNoTracking()
                .Where(x => x.Activo)
                .Select(x => new GetTipoComidaDto
                {
                    TipoComidaId = x.TipoComidaId,
                    Nombre = x.Nombre
                }).ToListAsync(cancellationToken);  
        }
    }
}
