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

namespace Csd.Comisiones.Application.Features.Solicitudes.SendSolicitud
{
    public class EnviarSolicitudCommandHandler : IRequestHandler<EnviarSolicitudCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly ISolicitudRepository _solicitudRepository;
        private readonly IEmailService _emailService;
        public EnviarSolicitudCommandHandler(
            IApplicationDbContext context,
            ISolicitudRepository solicitudRepository,
            IEmailService emailService)
        {
            _context = context;
            _solicitudRepository = solicitudRepository;
            _emailService = emailService;
        }

        public async Task<Unit> Handle(EnviarSolicitudCommand request, CancellationToken cancellationToken)
        {
            var solicitud = await _context.Solicitud
                .Include(x => x.Autorizaciones)
                .FirstOrDefaultAsync(x => x.SolicitudId == request.SolicitudId, cancellationToken);

            if (solicitud == null)
                throw new Exception("Solicitud no encontrada");

            if (solicitud.EstatusSolicitudId != (int)EstatusSolicitudEnum.Borrador)
                throw new Exception("Solo se pueden enviar solicitudes en borrador");

            var autorizadores = await _context.Autorizador
                .Where(x => x.ObraId == solicitud.ObraId && x.Activo)
                .ToListAsync(cancellationToken);

            if (!autorizadores.Any())
                throw new Exception("No hay autorizadores configurados");

            foreach (var autorizador in autorizadores)
            {
                var autorizacion = new SolicitudAutorizacion(
                    solicitudId: solicitud.SolicitudId,
                    autorizadorId: autorizador.AutorizadorId
                );

                solicitud.Autorizaciones.Add(autorizacion);
            }

            // Cambiar estatus
            solicitud.Enviar();

            await _solicitudRepository.UpdateAsync(solicitud);
            await _solicitudRepository.SaveChangesAsync(cancellationToken);

            var empleados = await _context.Empleado
                .Where(e => autorizadores
                    .Select(a => a.EmpleadoId)
                    .Contains(e.EmpleadoId))
                .Select(e => new { e.EmpleadoId, e.Correo })
                .ToListAsync(cancellationToken);

            var empleadosDict = empleados
                .Where(e => !string.IsNullOrEmpty(e.Correo))
                .ToDictionary(e => e.EmpleadoId, e => e.Correo!);

            foreach (var autorizador in autorizadores)
            {
                if (empleadosDict.TryGetValue(autorizador.EmpleadoId, out var correo))
                {
                    await _emailService.SendSolicitudPendienteAsync(
                        correo,
                        solicitud.Folio,
                        solicitud.ObraId.ToString(),
                        solicitud.FechaInicio,
                        solicitud.FechaFin
                    );
                }
            }

            return Unit.Value;
        }
    }
}
