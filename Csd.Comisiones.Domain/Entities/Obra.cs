using Csd.Comisiones.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Domain.Entities
{
    public class Obra : AuditableEntity
    {
        public int ObraId { get; private set; }
        public string Nombre { get; private set; } = string.Empty;
        public string Descripcion { get; private set; } = string.Empty;
        public bool Activo { get; private set; }

        private Obra() { }

        public Obra(
            string nombre,
            string descripcion)
        {
            Nombre = nombre;
            Descripcion = descripcion;
            Activo = true;
        }
    }
}
