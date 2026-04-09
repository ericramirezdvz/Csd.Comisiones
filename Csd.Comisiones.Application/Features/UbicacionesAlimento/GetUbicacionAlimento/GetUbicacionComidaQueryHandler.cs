using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Application.Features.TiposComida.GetTipoComida;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.UbicacionesAlimento.GetUbicacionAlimento
{
    public class GetUbicacionComidaQueryHandler : IRequestHandler<GetUbicacionAlimentoQuery, List<GetUbicacionAlimentoDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetUbicacionComidaQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<GetUbicacionAlimentoDto>> Handle(GetUbicacionAlimentoQuery request, CancellationToken cancellationToken)
        {
            return await _context.UbicacionAlimento
                .AsNoTracking()
                .Where(x => x.Activo)
                .Select(x => new GetUbicacionAlimentoDto
                {
                    UbicacionAlimentoId = x.UbicacionAlimentoId,
                    Nombre = x.Nombre
                }).ToListAsync(cancellationToken);
        }
    }
}
