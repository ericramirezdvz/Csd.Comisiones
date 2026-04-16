using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Application.Features.Obras.GetObras;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.MotivosSolicitud.GetMotivosSolicitud
{
    public class GetMotivoSolicitudQueryHandler : IRequestHandler<GetMotivoSolicitudQuery, List<GetMotivoSolicitudDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetMotivoSolicitudQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<GetMotivoSolicitudDto>> Handle(GetMotivoSolicitudQuery request, CancellationToken cancellationToken)
        {
            return await _context.MotivoSolicitud
                .Select(o => new GetMotivoSolicitudDto
                {
                    Id = o.MotivoSolicitudId,
                    Nombre = o.Nombre
                })
                .ToListAsync(cancellationToken);
        }
    }
}
