using Csd.Comisiones.Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.TrackingSolicitud
{
    public class SeguimientoSolicitudQueryHandler
    : IRequestHandler<SeguimientoSolicitudQuery, List<SolicitudSeguimientoDto>>
    {
        private readonly IApplicationDbContext _context;

        public SeguimientoSolicitudQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SolicitudSeguimientoDto>> Handle(SeguimientoSolicitudQuery request, CancellationToken cancellationToken)
        {
            var seguimientos = await _context.SolicitudSeguimiento
            .Where(x => x.SolicitudId == request.SolicitudId)
            .Include(x => x.Estatus)
            .OrderByDescending(x => x.Fecha)
            .Select(x => new SolicitudSeguimientoDto
            {
                Id = x.SolicitudSeguimientoId,
                EstatusId = x.EstatusSolicitudId,
                Estatus = x.Estatus.Nombre, // ajusta si se llama diferente
                Comentarios = x.Comentarios,
                Fecha = x.Fecha
            })
            .ToListAsync(cancellationToken);

            return seguimientos;
        }
    }
}
