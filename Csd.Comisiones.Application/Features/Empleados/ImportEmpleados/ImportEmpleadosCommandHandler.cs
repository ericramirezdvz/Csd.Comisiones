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
            var importados = _fileParserService
                .LeerNumerosEmpleadoAsync(request.FileStream, request.FileName);

            var numerosEmpleado = importados
                .Where(x => !string.IsNullOrWhiteSpace(x.NumeroEmpleado))
                .Select(x => x.NumeroEmpleado!)
                .Distinct()
                .ToList();

            var empleadosDb = await _context.Empleado
                .Where(e => numerosEmpleado.Contains(e.NumeroEmpleado))
                .ToListAsync(cancellationToken);

            var empleadosDict = empleadosDb.ToDictionary(e => e.NumeroEmpleado);

            var resultado = new List<EmpleadoDto>();

            foreach (var item in importados)
            {
                //  EMPLEADO INTERNO
                if (!string.IsNullOrWhiteSpace(item.NumeroEmpleado) &&
                    empleadosDict.TryGetValue(item.NumeroEmpleado, out var emp))
                {
                    resultado.Add(new EmpleadoDto
                    {
                        EmpleadoId = emp.EmpleadoId,
                        NumeroEmpleado = emp.NumeroEmpleado,
                        NombreCompleto = emp.NombreCompleto,
                        Correo = emp.Correo,
                        EsExterno = false
                    });
                }
                // EXTERNO
                else if (!string.IsNullOrWhiteSpace(item.NombreExterno))
                {
                    resultado.Add(new EmpleadoDto
                    {
                        EmpleadoId = null,
                        NumeroEmpleado = null,
                        NombreCompleto = item.NombreExterno,
                        Correo = null,
                        EsExterno = true
                    });
                }
            }

            return resultado;
        }
    }
}
