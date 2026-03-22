using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Empleados.GetEmpleadoByNumero
{
    public class EmpleadoDto
    {
        public int EmpleadoId { get; set; }
        public string NumeroEmpleado { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
    }
}
