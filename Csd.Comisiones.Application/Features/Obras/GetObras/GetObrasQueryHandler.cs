using Csd.Comisiones.Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Obras.GetObras
{
    public class GetObrasQueryHandler : IRequestHandler<GetObrasQuery, List<ObraDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetObrasQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ObraDto>> Handle(GetObrasQuery request, CancellationToken cancellationToken)
        {
            return await _context.Obra
                .Select(o => new ObraDto
                {
                    Id = o.ObraId,
                    Nombre = o.Nombre,
                    Empresa = o.EmpresaId != null ? o.EmpresaId.ToString() : ""
                })
                .ToListAsync(cancellationToken);
        }
    }
}
