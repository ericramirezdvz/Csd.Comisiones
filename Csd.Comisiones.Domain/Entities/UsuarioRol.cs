using Csd.Comisiones.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Domain.Entities
{
    public class UsuarioRol : AuditableEntity
    {
        public int UsuarioId { get; private set; }
        public int RolId { get; private set; }

        public Usuario Usuario { get; private set; } = null!;
        public Rol Rol { get; private set; } = null!;
    }
}
