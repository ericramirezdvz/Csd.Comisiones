using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Proveedores.SendProveedores
{
    public class ProveedorDetalleDto
    {
        public string NombreEmpleado { get; set; } = string.Empty;
        public string TipoServicio { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
