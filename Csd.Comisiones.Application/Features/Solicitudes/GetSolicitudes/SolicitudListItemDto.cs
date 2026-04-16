using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.GetSolicitudes
{
    public class SolicitudListItemDto
    {
        public int SolicitudId { get; set; }
        public string Folio { get; set; } = string.Empty;

        public int ObraId { get; set; }
        public string ObraNombre { get; set; } = string.Empty;

        public int AreaId { get; set; }
        public string AreaNombre { get; set; } = string.Empty;

        public int EstatusSolicitudId { get; set; }
        public string EstatusNombre { get; set; } = string.Empty;

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public string? Comentarios { get; set; }

        public DateTime FechaCreacion { get; set; }

        public int TotalEmpleados { get; set; }
        public int TotalHoteles { get; set; }
        public int TotalComidas { get; set; }
    }
}
