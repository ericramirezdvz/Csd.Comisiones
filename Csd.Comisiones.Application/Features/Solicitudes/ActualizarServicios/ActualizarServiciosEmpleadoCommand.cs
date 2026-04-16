using MediatR;
using System.Collections.Generic;

namespace Csd.Comisiones.Application.Features.Solicitudes.ActualizarServicios
{
    public class ActualizarServiciosEmpleadoCommand : IRequest<ActualizarServiciosResult>
    {
        public int SolicitudId { get; set; }
        public int EmpleadoId { get; set; }
        public List<DiaServicioDto> Dias { get; set; } = new();
        public int? ProveedorDesayunoId { get; set; }
        public int? ProveedorComidaId { get; set; }
        public int? ProveedorCenaId { get; set; }
    }

    public class ActualizarServiciosResult
    {
        public bool ProveedoresNotificados { get; set; }
        public List<string> ProveedoresAfectados { get; set; } = new();
    }
}
