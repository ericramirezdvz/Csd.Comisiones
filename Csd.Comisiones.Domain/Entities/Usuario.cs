using Csd.Comisiones.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Domain.Entities
{
    public class Usuario : AuditableEntity
    {
        public int UsuarioId { get; private set; }
        public string Username { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public bool Activo { get; private set; }

        private readonly List<UsuarioRol> _roles = new();
        public IReadOnlyCollection<UsuarioRol> Roles => _roles;

        private Usuario() { }

        public Usuario(string username, string email)
        {
            Username = username;
            Email = email;
            Activo = true;
        }
    }
}
