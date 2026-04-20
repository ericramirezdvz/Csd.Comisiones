using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.GetSolicitudById
{
    public class SolicitudDetalleDto
    {
        public int SolicitudId { get; set; }
        public string Folio { get; set; } = string.Empty;

        public int ObraId { get; set; }
        public string ObraNombre { get; set; } = string.Empty;

        public int AreaId { get; set; }
        public string AreaNombre { get; set; } = string.Empty;
        public int? CiudadId { get; set; }

        public int EstatusSolicitudId { get; set; }
        public string EstatusNombre { get; set; } = string.Empty;

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaCreacion { get; set; }

        public string? Comentarios { get; set; }

        public int? MotivoSolicitudId { get; set; }
        public string? MotivoSolicitudNombre { get; set; }

        public List<EmpleadoSolicitudDto> Empleados { get; set; } = new();
    }
}
