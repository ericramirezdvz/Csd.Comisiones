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
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int AreaId { get; set; }
        public int EstatusSolicitudId { get; set; }

        public List<EmpleadoSolicitudDto> Empleados { get; set; } = new();
    }
}
