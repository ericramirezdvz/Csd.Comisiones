using Csd.Comisiones.Application.Common.Models;
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
                .Include(x => x.Empleados)
                    .ThenInclude(e => e.Empleado)
                .Include(x => x.Empleados)
                    .ThenInclude(e => e.Hoteles)
                .Include(x => x.Empleados)
                    .ThenInclude(e => e.Comidas)
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

            // CALCULAR LOS PRECIOS MAXIMOS PARA CADA SERVICIO
            var preciosMaximos = await _context.ProveedorServicio
                .GroupBy(x => x.TipoServicio)
                .Select(g => new
                {
                    TipoServicio = g.Key,
                    PrecioMaximo = g.Max(x => x.Precio)
                })
                .ToDictionaryAsync(x => x.TipoServicio, x => x.PrecioMaximo, cancellationToken);

            decimal GetPrecioMax(TipoServicioEnum tipoServicio) =>
            preciosMaximos.TryGetValue(tipoServicio, out var precio) ? precio : 0;

            // ARMANDO EL EMAIL PARA LOS AUTORIZADORES
            var empleadosEmail = solicitud.Empleados
            .Select(e =>
            {
                var hotelesActivos = e.Hoteles
                    .Where(h => h.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada);

                var comidasActivas = e.Comidas
                    .Where(c => c.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada);

                // HOTEL
                //var totalHotel = hotelesActivos.Sum(h =>
                //{
                //    var noches = (h.FechaFin - h.FechaInicio).Days;
                //    return noches * h.PrecioUnitario;
                //});
                var totalHotel = hotelesActivos.Sum(h =>
                {
                    var noches = (h.FechaFin - h.FechaInicio).Days;

                    var tipoServicio = h.TipoHabitacionId == (int)TipoServicioEnum.HabitacionSencilla
                        ? (int)TipoServicioEnum.HabitacionSencilla
                        : (int)TipoServicioEnum.HabitacionDoble;

                    var precioMax = GetPrecioMax((TipoServicioEnum)tipoServicio);

                    return noches * precioMax;
                });


                // COMIDA
                //var totalComida = comidasActivas.Sum(c => c.PrecioUnitario);
                var precioAlimentoMax = GetPrecioMax(TipoServicioEnum.Alimento);
                var totalComida = comidasActivas.Count() * precioAlimentoMax;
                
                var total = totalHotel + totalComida;

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

            // Obtener correos de autorizadores
            var empleados = await _context.Empleado
                .Where(e => autorizadores
                    .Select(a => a.EmpleadoId)
                    .Contains(e.EmpleadoId))
                .Select(e => new { e.EmpleadoId, e.Correo })
                .ToListAsync(cancellationToken);

            var empleadosDict = empleados
                .Where(e => !string.IsNullOrEmpty(e.Correo))
                .ToDictionary(e => e.EmpleadoId, e => e.Correo!);

            var tasks = autorizadores
            .Where(a => empleadosDict.ContainsKey(a.EmpleadoId))
            .Select(a =>
            {
                var correo = empleadosDict[a.EmpleadoId];

                return _emailService.SendSolicitudPendienteAsync(
                    solicitud.SolicitudId,
                    a.AutorizadorId,
                    correo,
                    solicitud.Folio,
                    solicitud.ObraId.ToString(),
                    solicitud.FechaInicio,
                    solicitud.FechaFin,
                    empleadosEmail
                );
            });

            await Task.WhenAll(tasks);

            return Unit.Value;
        }
    }
}
