using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.CreateSolicitud
{
    public class CreateSolicitudCommand : IRequest<int>
    {
        public string Folio { get; set; } = string.Empty;

        public int AreaId { get; set; }
        public int ObraId { get; set; }
        public int CiudadId { get; set; }
        public int SolicitanteId { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public string? Comentarios { get; set; }

        public int? MotivoSolicitudId { get; set; }

        public List<CreateSolicitudEmpleadoDto> Empleados { get; set; } = new();
    }
}
