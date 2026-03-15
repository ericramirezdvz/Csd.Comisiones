using Csd.Comisiones.Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.EstatusSolicitudes.GetEstatusSolicitud
{
    public class GetEstatusSolicitudQueryHandler : IRequestHandler<GetEstatusSolicitudQuery, List<GetEstatusSolicitudDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetEstatusSolicitudQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<GetEstatusSolicitudDto>> Handle(GetEstatusSolicitudQuery request, CancellationToken cancellationToken)
        {
            return await _context.EstatusSolicitud
                .AsNoTracking()
                .Where(e => e.Activo)
                .Select(e => new GetEstatusSolicitudDto
                {
                    EstatusSolicitudId = e.EstatusSolicitudId,
                    Nombre = e.Nombre
                })
                .ToListAsync(cancellationToken);
        }
    }
}
