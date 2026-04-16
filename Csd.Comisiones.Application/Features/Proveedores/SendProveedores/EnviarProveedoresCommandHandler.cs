using Csd.Comisiones.Application.Contracts.Infrastructure;
using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Domain.Entities;
using Csd.Comisiones.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Proveedores.SendProveedores
{
    public class EnviarProveedoresCommandHandler : IRequestHandler<EnviarProveedoresCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public EnviarProveedoresCommandHandler(
            IApplicationDbContext context,
            IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<Unit> Handle(EnviarProveedoresCommand request, CancellationToken cancellationToken)
        {
            var solicitud = await _context.Solicitud
                .Include(x => x.Empleados)
                    .ThenInclude(e => e.Empleado)
                .Include(x => x.Empleados)
                    .ThenInclude(e => e.Hoteles)
                        .ThenInclude(h => h.Proveedor)
                .Include(x => x.Empleados)
                    .ThenInclude(e => e.Comidas)
                        .ThenInclude(c => c.Proveedor)
                .FirstOrDefaultAsync(x => x.SolicitudId == request.SolicitudId, cancellationToken);

            if (solicitud == null)
                throw new Exception("Solicitud no encontrada");

            // Invalidar tokens anteriores para esta solicitud
            var tokensAnteriores = await _context.RespuestaProveedor
                .Where(r => r.SolicitudId == request.SolicitudId && r.Vigente)
                .ToListAsync(cancellationToken);

            foreach (var tokenAnterior in tokensAnteriores)
            {
                tokenAnterior.Invalidar();
            }

            // Agrupar por proveedor (solo servicios no cancelados con proveedor asignado)
            var serviciosHotel = solicitud.Empleados
                .SelectMany(e => e.Hoteles
                    .Where(h => h.ProveedorId != null && h.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada)
                    .Select(h => new
                    {
                        Proveedor = h.Proveedor,
                        Empleado = e,
                        Tipo = "Hotel",
                        FechaInicio = h.FechaInicio,
                        FechaFin = h.FechaFin,
                        Hotel = (SolicitudHotel?)h,
                        Comida = (SolicitudComida?)null
                    }));

            var serviciosComida = solicitud.Empleados
                .SelectMany(e => e.Comidas
                    .Where(c => c.ProveedorId != null && c.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada)
                    .Select(c => new
                    {
                        Proveedor = c.Proveedor,
                        Empleado = e,
                        Tipo = "Comida",
                        FechaInicio = c.FechaInicio,
                        FechaFin = c.FechaFin,
                        Hotel = (SolicitudHotel?)null,
                        Comida = (SolicitudComida?)c
                    }));

            var proveedores = serviciosHotel
                .Concat(serviciosComida)
                .GroupBy(x => x.Proveedor.ProveedorId)
                .ToList();

            // Generar token y enviar correo por proveedor
            foreach (var grupo in proveedores)
            {
                var proveedor = grupo.First().Proveedor;

                // Transicionar servicios a EnProceso
                foreach (var servicio in grupo)
                {
                    servicio.Hotel?.EnviarAProveedor();
                    servicio.Comida?.EnviarAProveedor();
                }

                // Crear token de respuesta
                var respuesta = new RespuestaProveedor(request.SolicitudId, proveedor.ProveedorId);
                _context.RespuestaProveedor.Add(respuesta);

                if (string.IsNullOrEmpty(proveedor.Correo))
                    continue;

                var detalles = grupo.Select(x => new ProveedorDetalleDto
                {
                    NombreEmpleado = x.Empleado.Empleado.NombreCompleto,
                    TipoServicio = x.Tipo,
                    FechaInicio = x.FechaInicio,
                    FechaFin = x.FechaFin
                }).ToList();

                await _emailService.SendSolicitudProveedorAsync(
                    proveedor.Correo,
                    proveedor.Nombre,
                    solicitud.Folio,
                    detalles,
                    respuesta.Token
                );
            }

            await ((DbContext)_context).SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
