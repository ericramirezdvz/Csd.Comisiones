using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Proveedores.SendProveedores
{
    public class ProveedorDetalleDto
    {
        public string TipoServicio { get; set; } = string.Empty;
        public int Cantidad { get; set; } = 1;
        /// <summary>
        /// Número de días/noches del periodo. Para hospedaje = noches, para alimentos = días de servicio.
        /// </summary>
        public int Dias { get; set; } = 1;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * Dias * PrecioUnitario;

        /// <summary>
        /// Nombres de empleados asociados a esta línea (para referencia visual).
        /// </summary>
        public List<string> Empleados { get; set; } = new();
    }
}
