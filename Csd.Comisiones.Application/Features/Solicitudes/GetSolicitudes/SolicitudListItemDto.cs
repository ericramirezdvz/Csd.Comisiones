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

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public int TotalEmpleados { get; set; }
        public int TotalHoteles { get; set; }
        public int TotalComidas { get; set; }
    }
}
