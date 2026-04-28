using Csd.Comisiones.Application.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.GetSolicitudById
{
    public class GetSolicitudByIdQueryHandler : IRequestHandler<GetSolicitudByIdQuery, SolicitudDetalleDto>
    {
        private readonly IApplicationDbContext _context;

        public GetSolicitudByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SolicitudDetalleDto> Handle(GetSolicitudByIdQuery request, CancellationToken cancellationToken)
        {
            var solicitud = await _context.Solicitud
                .Where(s => s.SolicitudId == request.SolicitudId)
                .Select(s => new SolicitudDetalleDto
                {
                    SolicitudId = s.SolicitudId,
                    Folio = s.Folio,

                    ObraId = s.ObraId,
                    ObraNombre = s.Obra.Nombre,

                    AreaId = s.AreaId,
                    AreaNombre = s.Area.Nombre,
                    CiudadId = s.CiudadId,

                    EstatusSolicitudId = s.EstatusSolicitudId,
                    EstatusNombre = s.Estatus.Nombre,

                    FechaInicio = s.FechaInicio,
                    FechaFin = s.FechaFin,
                    FechaCreacion = s.FechaCreacion,

                    Comentarios = s.Comentarios,

                    MotivoSolicitudId = s.MotivoSolicitudId,
                    MotivoSolicitudNombre = s.MotivoSolicitud != null ? s.MotivoSolicitud.Nombre : null,

                    Empleados = s.Empleados.Select(e => new EmpleadoSolicitudDto
                    {
                        EmpleadoId = e.EmpleadoId.HasValue ? e.EmpleadoId.Value : 0,
                        NombreCompleto = e.Empleado.NombreCompleto,
                        NumeroEmpleado = e.Empleado.NumeroEmpleado,
                        AreaId = e.Empleado.AreaId,
                        Correo = e.Empleado.Correo,
                        TipoAsignacion = (int)e.TipoAsignacion,
                        MontoPago = e.MontoPago,
                        FechaInicio = e.FechaInicio,
                        FechaFin = e.FechaFin,

                        Hoteles = e.Hoteles.Select(h => new HotelSolicitudDto
                        {
                            SolicitudHotelId = h.SolicitudHotelId,
                            ProveedorId = h.ProveedorId,
                            TipoHabitacionId = h.TipoHabitacionId,
                            FechaInicio = h.FechaInicio,
                            FechaFin = h.FechaFin,
                            PrecioUnitario = h.PrecioUnitario,
                            EstatusDetalleId = h.EstatusDetalleId
                        }).ToList(),

                        Comidas = e.Comidas.Select(c => new ComidaSolicitudDto
                        {
                            SolicitudComidaId = c.SolicitudComidaId,
                            ProveedorId = c.ProveedorId,
                            TipoComidaId = c.TipoComidaId,
                            FechaInicio = c.FechaInicio,
                            FechaFin = c.FechaFin,
                            PrecioUnitario = c.PrecioUnitario,
                            EstatusDetalleId = c.EstatusDetalleId
                        }).ToList()
                    }).ToList()
                }).FirstOrDefaultAsync(cancellationToken);

            return solicitud;
        }
    }
}
