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

            // CALCULAR LOS PRECIOS MAXIMOS PARA CADA SERVICIO (solo proveedores de la ciudad de la comisión)
            var preciosMaximos = await _context.ProveedorServicio
                .Where(x => x.Proveedor.CiudadId == solicitud.CiudadId && x.Proveedor.Activo)
                .GroupBy(x => x.TipoServicio)
                .Select(g => new
                {
                    TipoServicio = g.Key,
                    PrecioMaximo = g.Max(x => x.Precio)
                })
                .ToDictionaryAsync(x => x.TipoServicio, x => x.PrecioMaximo, cancellationToken);

            decimal GetPrecioMax(TipoServicioEnum tipoServicio) {
                // Solo para hospedaje en Carmen
                if (solicitud.CiudadId == 1 &&
                    (tipoServicio == TipoServicioEnum.HabitacionSencilla ||
                     tipoServicio == TipoServicioEnum.HabitacionDoble))
                {
                    return _context.ProveedorServicio
                        .Where(x => x.ProveedorId == 5 && x.TipoServicio == tipoServicio)
                        .Select(x => x.Precio)
                        .FirstOrDefault();
                }

                return preciosMaximos.TryGetValue(tipoServicio, out var precio)
                    ? precio
                    : 0;
            }

            // Determinar tipos de servicio desde metadata en comentarios
            var comentarios = solicitud.Comentarios ?? "";
            var metaMatch = System.Text.RegularExpressions.Regex.Match(comentarios, @"##META:tiposServicio=([A-Z,]+)");
            var tiposServicio = metaMatch.Success
                ? metaMatch.Groups[1].Value.Split(',').ToHashSet()
                : new HashSet<string> { "HOSPEDAJE", "COMIDA" }; // default

            // Fallback: formato legacy ##META:tipoServicio=HOSPEDAJE_COMIDA
            if (!metaMatch.Success)
            {
                var legacyMatch = System.Text.RegularExpressions.Regex.Match(comentarios, @"##META:tipoServicio=(HOSPEDAJE_COMIDA|HOSPEDAJE|COMIDA|PAGO)");
                if (legacyMatch.Success)
                {
                    var val = legacyMatch.Groups[1].Value;
                    tiposServicio = val == "HOSPEDAJE_COMIDA"
                        ? new HashSet<string> { "HOSPEDAJE", "COMIDA" }
                        : new HashSet<string> { val };
                }
            }

            var incluyeHospedaje = tiposServicio.Contains("HOSPEDAJE");
            var incluyeComida = tiposServicio.Contains("COMIDA");

            // ARMANDO EL EMAIL PARA LOS AUTORIZADORES
            var empleadosEmail = solicitud.Empleados
            .Select(e =>
            {
                // PAGO DIRECTO: usar el monto exacto, sin estimación de servicios
                if (e.TipoAsignacion == TipoAsignacionEnum.Pago)
                {
                    return new EmpleadoEmailDto
                    {
                        Nombre = e.Empleado.NombreCompleto,
                        FechaInicio = e.FechaInicio,
                        FechaFin = e.FechaFin,
                        RequiereHotel = false,
                        Desayuno = false,
                        Almuerzo = false,
                        Cena = false,
                        Total = e.MontoPago ?? 0,
                        EsPago = true
                    };
                }

                // SERVICIOS: estimar con precios máximos de la ciudad
                var hotelesActivos = e.Hoteles
                    .Where(h => h.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada);

                var comidasActivas = e.Comidas
                    .Where(c => c.EstatusDetalleId != (int)EstatusDetalleEnum.Cancelada);

                var tieneHoteles = hotelesActivos.Any();
                var tieneComidas = comidasActivas.Any();

                decimal totalHotel;
                decimal totalComida;
                bool requiereHotel;
                bool desayuno;
                bool almuerzo;
                bool cena;

                if (tieneHoteles || tieneComidas)
                {
                    // Ya hay servicios asignados: calcular con datos reales
                    totalHotel = hotelesActivos.Sum(h =>
                    {
                        var noches = (h.FechaFin - h.FechaInicio).Days;
                        var tipoServicio = h.TipoHabitacionId == (int)TipoServicioEnum.HabitacionSencilla
                            ? (int)TipoServicioEnum.HabitacionSencilla
                            : (int)TipoServicioEnum.HabitacionDoble;
                        var precioMax = GetPrecioMax((TipoServicioEnum)tipoServicio);
                        return noches * precioMax;
                    });

                    var precioAlimentoMax = GetPrecioMax(TipoServicioEnum.Alimento);
                    totalComida = comidasActivas.Count() * precioAlimentoMax;

                    requiereHotel = tieneHoteles;
                    desayuno = comidasActivas.Any(c => c.TipoComidaId == (int)TipoComidaEnum.Desayuno);
                    almuerzo = comidasActivas.Any(c => c.TipoComidaId == (int)TipoComidaEnum.Almuerzo);
                    cena = comidasActivas.Any(c => c.TipoComidaId == (int)TipoComidaEnum.Cena);
                }
                else
                {
                    // Sin servicios aún: estimar según tipoServicio de la comisión
                    var dias = (e.FechaFin - e.FechaInicio).Days;
                    var noches = Math.Max(dias - 1, 0);

                    totalHotel = 0;
                    requiereHotel = incluyeHospedaje;
                    if (incluyeHospedaje && noches > 0)
                    {
                        var precioHotel = GetPrecioMax(TipoServicioEnum.HabitacionSencilla);
                        totalHotel = noches * precioHotel;
                    }

                    totalComida = 0;
                    desayuno = incluyeComida;
                    almuerzo = incluyeComida;
                    cena = incluyeComida;
                    if (incluyeComida && dias > 0)
                    {
                        var precioAlimento = GetPrecioMax(TipoServicioEnum.Alimento);
                        // 3 comidas por día (desayuno, comida, cena)
                        totalComida = dias * 3 * precioAlimento;
                    }
                }

                var total = totalHotel + totalComida;

                return new EmpleadoEmailDto
                {
                    Nombre = e.Empleado.NombreCompleto,
                    FechaInicio = e.FechaInicio,
                    FechaFin = e.FechaFin,

                    RequiereHotel = requiereHotel,

                    Desayuno = desayuno,
                    Almuerzo = almuerzo,
                    Cena = cena,

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
