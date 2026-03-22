using Csd.Comisiones.Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Empleados.GetEmpleadoById
{
    public   class GetEmpleadoByIdQueryHandler : IRequestHandler<GetEmpleadoByIdQuery, EmpleadoDto>
    {
        private readonly IApplicationDbContext _context;

        public GetEmpleadoByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EmpleadoDto> Handle(GetEmpleadoByIdQuery request, CancellationToken cancellationToken)
        {
            var empleado = await _context.Empleado
                .Where(e => e.EmpleadoId == request.EmpleadoId)
                .Select(e => new EmpleadoDto
                {
                    EmpleadoId = e.EmpleadoId,
                    NumeroEmpleado = e.NumeroEmpleado,
                    Nombre = e.NombreCompleto,
                    Correo = e.Correo
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (empleado == null)
            {
                throw new Exception($"Empleado con id {request.EmpleadoId} no existe");
            }

            return empleado;
        }
    }
}
