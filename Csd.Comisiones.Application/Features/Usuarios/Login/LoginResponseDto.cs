using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Usuarios.Login
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public string? Email { get; set; }
        public string? NombreCompleto { get; set; }
        public int UsuarioId { get; set; }
    }
}
