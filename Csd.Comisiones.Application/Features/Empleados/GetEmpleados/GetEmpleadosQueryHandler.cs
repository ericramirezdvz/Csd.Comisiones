using Csd.Comisiones.Application.Common.Extensions;
using Csd.Comisiones.Application.Common.Models;
using Csd.Comisiones.Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Empleados.GetEmpleados
{
    public class GetEmpleadosQueryHandler : IRequestHandler<GetEmpleadosQuery, PagedResult<EmpleadoDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetEmpleadosQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PagedResult<EmpleadoDto>> Handle(GetEmpleadosQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Empleado
                .AsNoTracking()
                .Where(e => e.Activo);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(e =>
                    e.NombreCompleto.ToLower().Contains(search) ||
                    e.NumeroEmpleado.ToLower().Contains(search));
            }

            var projected = query.Select(e => new EmpleadoDto
            {
                EmpleadoId = e.EmpleadoId,
                NumeroEmpleado = e.NumeroEmpleado,
                Nombre = e.NombreCompleto,
                Correo = e.Correo
            });

            var totalCount = await projected.CountAsync(cancellationToken);

            return await projected.ToPagedResultAsync(
                request.PageNumber,
                request.PageSize,
                cancellationToken
                );
        }
    }
}
