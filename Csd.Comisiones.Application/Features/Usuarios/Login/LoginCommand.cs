using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Usuarios.Login
{
    public class LoginCommand : IRequest<LoginResponseDto>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
