using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Proveedores.GetProveedores
{
    public class ProveedorServicioDto
    {
        public int TipoServicioId { get; set; }
        public string TipoServicioNombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
    }

    public class ProveedorDto
    {
        public int ProveedorId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool ProporcionaHospedaje { get; set; }
        public bool ProporcionaAlimentos { get; set; }
        public List<ProveedorServicioDto> Servicios { get; set; } = new();
    }
}
