using Csd.Comisiones.Domain.Common;
using Csd.Comisiones.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Domain.Entities
{
    public class Proveedor : AuditableEntity
    {
        public int ProveedorId { get; private set; }

        public string Nombre { get; private set; } = string.Empty;

        public TipoProveedorEnum TipoProveedor { get; private set; }

        public int CiudadId { get; private set; }

        public bool ProporcionaHospedaje { get; private set; }
        public bool ProporcionaAlimentos { get; private set; }

        public bool Activo { get; private set; }
        public string? Correo { get; set; }

        public ICollection<ProveedorServicio> Servicios { get; private set; } = new List<ProveedorServicio>();

        private Proveedor() { }

        public Proveedor(
            string nombre,
            TipoProveedorEnum tipoProveedor,
            int ciudadId,
            bool proporcionaHospedaje,
            bool proporcionaAlimentos,
            string? correo)
        {
            Nombre = nombre;
            TipoProveedor = tipoProveedor;
            CiudadId = ciudadId;
            ProporcionaHospedaje = proporcionaHospedaje;
            ProporcionaAlimentos = proporcionaAlimentos;
            Activo = true;
            Correo = correo;
        }

        public void Desactivar()
        {
            Activo = false;
        }
    }
}
