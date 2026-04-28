using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Domain.Entities;
using Csd.Comisiones.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.UpdateSolicitud
{
    public class UpdateSolicitudCommandHandler
    : IRequestHandler<UpdateSolicitudCommand, Unit>
    {
        private readonly ISolicitudRepository _solicitudRepository;

        public UpdateSolicitudCommandHandler(ISolicitudRepository solicitudRepository)
        {
            _solicitudRepository = solicitudRepository;
        }

        public async Task<Unit> Handle(UpdateSolicitudCommand request, CancellationToken cancellationToken)
        {
            var solicitud = await _solicitudRepository.GetByIdAsync(request.SolicitudId);

            if (solicitud == null)
                throw new Exception("Solicitud no encontrada");

            solicitud.Actualizar(
                request.AreaId,
                request.ObraId,
                request.CiudadId,
                request.FechaInicio,
                request.FechaFin,
                request.Comentarios,
                request.MotivoSolicitudId);

            solicitud.LimpiarEmpleados();

            foreach (var empleadoDto in request.Empleados)
            {
                SolicitudEmpleado solicitudEmpleado;

                var esPago = empleadoDto.TipoAsignacion == (int)TipoAsignacionEnum.Pago;

                if (esPago)
                {
                    solicitudEmpleado = SolicitudEmpleado.CrearPago(
                        empleadoDto.EmpleadoId,
                        empleadoDto.NombreExterno,
                        empleadoDto.EsExterno,
                        empleadoDto.FechaInicio,
                        empleadoDto.FechaFin,
                        empleadoDto.MontoPago ?? 0,
                        (TipoPagoEnum)empleadoDto.TipoPago.Value);
                }
                else if (empleadoDto.EsExterno)
                {
                    solicitudEmpleado = SolicitudEmpleado.CrearExterno(
                        empleadoDto.NombreExterno!,
                        empleadoDto.FechaInicio,
                        empleadoDto.FechaFin);
                }
                else
                {
                    solicitudEmpleado = SolicitudEmpleado.CrearInterno(
                        empleadoDto.EmpleadoId!.Value,
                        empleadoDto.FechaInicio,
                        empleadoDto.FechaFin);
                }

                if (!esPago)
                {
                    foreach (var hotelDto in empleadoDto.Hoteles)
                    {
                        var hotel = new SolicitudHotel(
                            hotelDto.ProveedorId,
                            hotelDto.TipoHabitacionId,
                            hotelDto.FechaInicio,
                            hotelDto.FechaFin,
                            hotelDto.PrecioUnitario);

                        solicitudEmpleado.AgregarHotel(hotel);
                    }

                    foreach (var comidaDto in empleadoDto.Comidas)
                    {
                        var comida = new SolicitudComida(
                            comidaDto.TipoComidaId,
                            comidaDto.ProveedorId,
                            comidaDto.UbicacionId,
                            comidaDto.FechaInicio,
                            comidaDto.FechaFin,
                            comidaDto.PrecioUnitario);

                        solicitudEmpleado.AgregarComida(comida);
                    }
                }

                solicitud.AgregarEmpleado(solicitudEmpleado);
            }

            await _solicitudRepository.UpdateAsync(solicitud);
            await _solicitudRepository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
