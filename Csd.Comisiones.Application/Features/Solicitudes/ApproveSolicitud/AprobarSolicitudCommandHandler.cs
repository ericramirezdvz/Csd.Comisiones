using Csd.Comisiones.Application.Common.Models;
using Csd.Comisiones.Application.Contracts.Infrastructure;
using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.ApproveSolicitud
{
    public class AprobarSolicitudCommandHandler : IRequestHandler<AprobarSolicitudCommand, bool>
    {
        private readonly ISolicitudRepository _solicitudRepository;
        private readonly IApplicationDbContext _context;
        private readonly IEmailService _emailService;
        public AprobarSolicitudCommandHandler(ISolicitudRepository solicitudRepository, IApplicationDbContext context, IEmailService emailService)
        {
            _solicitudRepository = solicitudRepository;
            _context = context;
            _emailService = emailService;
        }

        public async Task<bool> Handle(AprobarSolicitudCommand request, CancellationToken cancellationToken)
        {
            var solicitud = await _context.Solicitud
                .Include(x => x.Empleados)
                    .ThenInclude(e => e.Empleado)
                .Include(x => x.Empleados)
                    .ThenInclude(e => e.Hoteles)
                .Include(x => x.Empleados)
                    .ThenInclude(e => e.Comidas)
                .FirstOrDefaultAsync(x => x.SolicitudId == request.SolicitudId, cancellationToken);

            if (solicitud == null)
                throw new Exception("Solicitud no encontrada");

            if (solicitud.EstatusSolicitudId != (int)EstatusSolicitudEnum.EnAutorizaciónPorResponsableDeObra)
                throw new Exception("Solo se pueden aprobar solicitudes en proceso de autorización");

            var solicitante = await _context.Empleado
                .FirstOrDefaultAsync(e => e.EmpleadoId == solicitud.SolicitanteId, cancellationToken);

            if (solicitante == null || string.IsNullOrEmpty(solicitante.Correo))
                throw new Exception("El solicitante no tiene correo configurado");

            solicitud.Aprobar();

            await _solicitudRepository.UpdateAsync(solicitud);

            await _solicitudRepository.SaveChangesAsync(cancellationToken);

            var empleadosEmail = solicitud.Empleados
                .Select(e =>
                {
                    var hotelesActivos = e.Hoteles
                        .Where(h => h.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada);

                    var comidasActivas = e.Comidas
                        .Where(c => c.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada);

                    var totalHotel = hotelesActivos.Sum(h =>
                    {
                        var noches = (h.FechaFin - h.FechaInicio).Days;
                        return noches * h.PrecioUnitario;
                    });

                    var totalComida = comidasActivas.Sum(c => c.PrecioUnitario);

                    var total = e.TipoAsignacion == TipoAsignacionEnum.Pago
                        ? e.MontoPago ?? 0
                        : totalHotel + totalComida;

                    return new EmpleadoEmailDto
                    {
                        Nombre = e.Empleado.NombreCompleto,
                        FechaInicio = e.FechaInicio,
                        FechaFin = e.FechaFin,

                        RequiereHotel = hotelesActivos.Any(),

                        Desayuno = comidasActivas.Any(c => c.TipoComidaId == (int)TipoComidaEnum.Desayuno),
                        Almuerzo = comidasActivas.Any(c => c.TipoComidaId == (int)TipoComidaEnum.Almuerzo),
                        Cena = comidasActivas.Any(c => c.TipoComidaId == (int)TipoComidaEnum.Cena),

                        Total = total
                    };
                })
                .ToList();

            await _emailService.SendSolicitudAprobadaAsync(
                solicitante.Correo,
                solicitud.Folio,
                solicitud.ObraId.ToString(),
                solicitud.FechaInicio,
                solicitud.FechaFin,
                empleadosEmail
            );

            return true;
        }
    }
}
