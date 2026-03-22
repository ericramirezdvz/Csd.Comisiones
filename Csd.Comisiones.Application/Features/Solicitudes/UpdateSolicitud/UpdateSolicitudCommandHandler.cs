using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Domain.Entities;
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
                request.FechaInicio,
                request.FechaFin,
                request.Comentarios);

            solicitud.LimpiarEmpleados();

            foreach (var empleadoDto in request.Empleados)
            {
                var solicitudEmpleado = new SolicitudEmpleado(
                    empleadoDto.EmpleadoId,
                    empleadoDto.FechaInicio,
                    empleadoDto.FechaFin);

                // Hoteles
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

                // Comidas
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

                solicitud.AgregarEmpleado(solicitudEmpleado);
            }

            await _solicitudRepository.UpdateAsync(solicitud);
            await _solicitudRepository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
