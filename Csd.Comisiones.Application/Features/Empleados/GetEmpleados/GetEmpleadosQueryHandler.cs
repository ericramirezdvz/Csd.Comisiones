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
                .Where(e => e.Activo)
                .Select(e => new EmpleadoDto
                {
                    EmpleadoId = e.EmpleadoId,
                    NumeroEmpleado = e.NumeroEmpleado,
                    Nombre = e.NombreCompleto,
                    Correo = e.Correo
                });

            var totalCount = await query.CountAsync(cancellationToken);

            return await query.ToPagedResultAsync(
                request.PageNumber,
                request.PageSize,
                cancellationToken
                );
        }
    }
}
