
using Csd.Comisiones.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Domain.Entities
{
    public class MotivoSolicitud : AuditableEntity
    {
        public int MotivoSolicitudId { get; private set; }
        public string Nombre { get; private set; } = string.Empty;
        public bool Activo { get; private set; }

        private MotivoSolicitud() { }

        public MotivoSolicitud(
            string nombre)
        {
            Nombre = nombre;
            Activo = true;
        }
    }
}
