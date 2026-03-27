using Csd.Comisiones.Domain.Common;
using Csd.Comisiones.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Domain.Entities
{
    public class ProveedorServicio : AuditableEntity
    {
        public int ProveedorServicioId { get; private set; }

        public int ProveedorId { get; private set; }

        public TipoServicioEnum TipoServicio { get; private set; }

        public decimal Precio { get; private set; }

        public Proveedor Proveedor { get; private set; } = null!;

        private ProveedorServicio() { }

        public ProveedorServicio(int proveedorId, TipoServicioEnum tipoServicio, decimal precio)
        {
            ProveedorId = proveedorId;
            TipoServicio = tipoServicio;
            Precio = precio;
        }
    }
}
