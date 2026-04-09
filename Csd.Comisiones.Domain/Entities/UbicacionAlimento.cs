using Csd.Comisiones.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Domain.Entities
{
    public class UbicacionAlimento : AuditableEntity
    {
        public int UbicacionAlimentoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; }

        private UbicacionAlimento() { }

        public UbicacionAlimento(string nombre)
        {
            Nombre = nombre;
            Activo = true;
        }
    }
}
