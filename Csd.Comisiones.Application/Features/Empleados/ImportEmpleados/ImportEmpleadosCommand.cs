using Csd.Comisiones.Application.Features.Empleados.GetEmpleados;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Empleados.ImportEmpleados
{
    public class ImportEmpleadosCommand : IRequest<List<EmpleadoDto>>
    {
        public Stream FileStream { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
    }
}
