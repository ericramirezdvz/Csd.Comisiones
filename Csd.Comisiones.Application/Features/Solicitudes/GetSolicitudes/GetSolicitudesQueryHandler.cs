using Csd.Comisiones.Application.Common.Extensions;
using Csd.Comisiones.Application.Common.Models;
using Csd.Comisiones.Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.GetSolicitudes
{
    public class GetSolicitudesQueryHandler
    : IRequestHandler<GetSolicitudesQuery, PagedResult<SolicitudListItemDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetSolicitudesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<SolicitudListItemDto>> Handle(
            GetSolicitudesQuery request,
            CancellationToken cancellationToken)
        {
            var query = _context.Solicitud
                .AsNoTracking()
                .Select(s => new SolicitudListItemDto
                {
                    SolicitudId = s.SolicitudId,
                    FechaInicio = s.FechaInicio,
                    FechaFin = s.FechaFin,

                    TotalEmpleados = s.Empleados.Count(),
                    TotalHoteles = s.Empleados.SelectMany(e => e.Hoteles).Count(),
                    TotalComidas = s.Empleados.SelectMany(e => e.Comidas).Count()
                });

            var totalCount = await query.CountAsync(cancellationToken);

            return await query.ToPagedResultAsync(
                request.PageNumber,
                request.PageSize,
                cancellationToken
                );

            //var items = await query
            //    .OrderByDescending(s => s.SolicitudId)
            //    .Skip((request.PageNumber - 1) * request.PageSize)
            //    .Take(request.PageSize)
            //    .ToListAsync(cancellationToken);

            //return new PagedResult<SolicitudListItemDto>
            //{
            //    Items = items,
            //    PageNumber = request.PageNumber,
            //    PageSize = request.PageSize,
            //    TotalCount = totalCount
            //};
        }
    }
}
