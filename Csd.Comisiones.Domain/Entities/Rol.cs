using Csd.Comisiones.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Domain.Entities
{
    public class Rol : AuditableEntity
    {
        public int RolId { get; private set; }
        public string Nombre { get; private set; } = string.Empty;
    }
}
