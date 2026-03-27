using Csd.Comisiones.Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.TiposHabitacion.GetTipoHabitacion
{
    public class GetTipoHabitacionQueryHandler : IRequestHandler<GetTipoHabitacionQuery, List<TipoHabitacionDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetTipoHabitacionQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TipoHabitacionDto>> Handle(GetTipoHabitacionQuery request, CancellationToken cancellationToken)
        {
            return await _context.TipoHabitacion
                .AsNoTracking()
                .Where(x => x.Activo)
                .Select(p => new TipoHabitacionDto
                {
                    TipoHabitacionId = p.TipoHabitacionId,
                    Nombre = p.Nombre
                })
                .ToListAsync(cancellationToken);
        }
    }
}
