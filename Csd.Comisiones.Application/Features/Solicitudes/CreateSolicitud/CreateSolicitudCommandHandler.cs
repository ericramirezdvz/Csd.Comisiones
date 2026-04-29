using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Domain.Entities;
using Csd.Comisiones.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.CreateSolicitud
{
    public class CreateSolicitudCommandHandler : IRequestHandler<CreateSolicitudCommand, int>
    {
        private readonly ISolicitudRepository _solicitudRepository;

        public CreateSolicitudCommandHandler(ISolicitudRepository solicitudRepository)
        {
            _solicitudRepository = solicitudRepository;
        }

        public async Task<int> Handle(CreateSolicitudCommand request, CancellationToken cancellationToken)
        {
            var solicitud = new Solicitud(
            request.Folio,
            request.AreaId,
            request.ObraId,
            request.CiudadId,
            request.SolicitanteId,
            request.FechaInicio,
            request.FechaFin,
            request.Comentarios);

            solicitud.MotivoSolicitudId = request.MotivoSolicitudId;

            foreach (var empleadoDto in request.Empleados)
            {
                SolicitudEmpleado solicitudEmpleado;

                if (empleadoDto.EsExterno)
                {
                    if (string.IsNullOrWhiteSpace(empleadoDto.NombreExterno))
                        throw new Exception("El nombre del externo es requerido");

                    if (empleadoDto.EmpleadoId.HasValue)
                        throw new Exception("Un externo no debe tener EmpleadoId");
                }
                else
                {
                    if (!empleadoDto.EmpleadoId.HasValue)
                        throw new Exception("El empleado es requerido");
                }

                var esPago = empleadoDto.TipoAsignacion == (int)TipoAsignacionEnum.Pago;

                if (esPago)
                {
                    if (!empleadoDto.TipoPago.HasValue)
                        throw new Exception("El tipo de pago es requerido");

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

            await _solicitudRepository.AddAsync(solicitud, cancellationToken);
            await _solicitudRepository.SaveChangesAsync(cancellationToken);

            return solicitud.SolicitudId;
        }
    }
}
