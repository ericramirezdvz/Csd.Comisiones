using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Empleados.ImportEmpleados
{
    public class EmpleadoDto
    {
        public int EmpleadoId { get; set; }
        public string NumeroEmpleado { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Correo { get; set; }
    }
}
