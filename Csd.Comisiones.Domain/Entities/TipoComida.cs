using Csd.Comisiones.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Domain.Entities
{
    public class TipoComida : AuditableEntity
    {
        public int TipoComidaId { get; private set; }
        public string Nombre { get; private set; } = string.Empty;
        public bool Activo { get; private set; }

        private TipoComida() { }

        public TipoComida(string nombre)
        {
            Nombre = nombre;
            Activo = true;
        }
    }
}
