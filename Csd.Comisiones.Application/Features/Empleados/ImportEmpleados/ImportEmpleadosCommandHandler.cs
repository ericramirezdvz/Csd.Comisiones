using Csd.Comisiones.Application.Contracts.Infrastructure;
using Csd.Comisiones.Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Empleados.ImportEmpleados
{
    public class ImportEmpleadosCommandHandler
    : IRequestHandler<ImportEmpleadosCommand, List<EmpleadoDto>>
    {
        private readonly IApplicationDbContext _context;

        private readonly IFileParserService _fileParserService;

        public ImportEmpleadosCommandHandler(
            IApplicationDbContext context,
            IFileParserService fileParserService)
        {
            _context = context;
            _fileParserService = fileParserService;
        }

        public async Task<List<EmpleadoDto>> Handle(
            ImportEmpleadosCommand request,
            CancellationToken cancellationToken)
        {
            var numerosEmpleado = await _fileParserService
                .LeerNumerosEmpleadoAsync(request.FileStream, request.FileName);

            numerosEmpleado = numerosEmpleado.Distinct().ToList();

            var empleados = await _context.Empleado
                .Where(e => numerosEmpleado.Contains(e.NumeroEmpleado))
                .ToListAsync(cancellationToken);

            return empleados.Select(e => new EmpleadoDto
            {
                EmpleadoId = e.EmpleadoId,
                NumeroEmpleado = e.NumeroEmpleado,
                NombreCompleto = e.NombreCompleto,
                Correo = e.Correo
            }).ToList();
        }
    }
}
