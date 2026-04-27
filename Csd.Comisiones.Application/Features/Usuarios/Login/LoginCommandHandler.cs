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

        // Mapeo de nombres de rol en BD → nombres que espera el frontend
        private static readonly Dictionary<string, string> _roleMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["administrador"] = "CAPITAL_HUMANO",
            ["solicitante"] = "SOLICITANTE",
        };

        private static string NormalizarRol(string rolBd) =>
            _roleMap.TryGetValue(rolBd, out var mapped) ? mapped : rolBd.ToUpperInvariant();

        public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Validar contra AD
            var isValid = await _adService.ValidateCredentials(request.Username, request.Password);

            if (!isValid)
                throw new Exception("Credenciales inválidas");

            // Normalizar username: extraer parte sin @dominio para búsqueda flexible
            var usernameRaw = request.Username.Trim();
            var usernameShort = usernameRaw.Contains('@')
                ? usernameRaw.Split('@')[0]
                : usernameRaw;

            // Buscar usuario en BD (por Username exacto o por la parte antes del @)
            var usuario = await _context.Usuario
                .Include(u => u.Roles)
                    .ThenInclude(r => r.Rol)
                .FirstOrDefaultAsync(u => u.Username == usernameRaw
                                       || u.Username == usernameShort, cancellationToken);

            if (usuario == null)
                throw new Exception("Usuario no registrado en el sistema");

            // Obtener roles normalizados
            var roles = usuario.Roles.Select(r => NormalizarRol(r.Rol.Nombre)).ToList();

            // Generar token
            var token = _jwt.GenerateToken(usuario.UsuarioId, usuario.Username, roles);

            // Derivar nombre legible del username (braulio.estrada → Braulio Estrada)
            var nombrePartes = usernameShort.Split('.', '_', '-');
            var nombreCompleto = string.Join(" ",
                nombrePartes.Select(p =>
                    string.IsNullOrEmpty(p) ? p : char.ToUpper(p[0]) + p[1..]));

            return new LoginResponseDto
            {
                Token = token,
                Username = usuario.Username,
                Roles = roles,
                Email = usuario.Email,
                NombreCompleto = nombreCompleto,
                UsuarioId = usuario.UsuarioId,
            };
        }
    }
}
