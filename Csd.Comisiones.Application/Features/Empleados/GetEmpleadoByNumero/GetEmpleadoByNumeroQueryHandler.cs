using Csd.Comisiones.Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Empleados.GetEmpleadoByNumero
{
    public class GetEmpleadoByNumeroQueryHandler : IRequestHandler<GetEmpleadoByNumeroQuery, EmpleadoDto>
    {
        private readonly IApplicationDbContext _context;

        public GetEmpleadoByNumeroQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EmpleadoDto> Handle(GetEmpleadoByNumeroQuery request, CancellationToken cancellationToken)
        {
            var empleado = await _context.Empleado
                .Where(e => e.NumeroEmpleado == request.NumeroEmpleado)
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
                throw new Exception($"Empleado con número {request.NumeroEmpleado} no existe");
            }

            return empleado;
        }
    }
}
