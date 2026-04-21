using Csd.Comisiones.Application.Contracts.Infrastructure;
using Csd.Comisiones.Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Usuarios.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
    {
        private readonly IActiveDirectoryService _adService;
        private readonly IApplicationDbContext _context;
        private readonly IJwtTokenGenerator _jwt;

        public LoginCommandHandler(
            IActiveDirectoryService adService,
            IApplicationDbContext context,
            IJwtTokenGenerator jwt)
        {
            _adService = adService;
            _context = context;
            _jwt = jwt;
        }

        public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Validar contra AD
            var isValid = await _adService.ValidateCredentials(request.Username, request.Password);

            if (!isValid)
                throw new Exception("Credenciales inválidas");

            // Buscar usuario en BD
            var usuario = await _context.Usuario
                .Include(u => u.Roles)
                    .ThenInclude(r => r.Rol)
                .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

            if (usuario == null)
                throw new Exception("Usuario no registrado en el sistema");

            // Obtener roles
            var roles = usuario.Roles.Select(r => r.Rol.Nombre).ToList();

            // Generar token
            var token = _jwt.GenerateToken(usuario.Username, roles);

            return new LoginResponseDto
            {
                Token = token,
                Username = usuario.Username,
                Roles = roles
            };
        }
    }
}
