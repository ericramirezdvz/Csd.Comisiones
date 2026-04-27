using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.CreateSolicitud
{
    public class CreateSolicitudEmpleadoDto
    {
        public int EmpleadoId { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public int TipoAsignacion { get; set; } = 1;

        public decimal? MontoPago { get; set; }

        public List<CreateSolicitudHotelDto> Hoteles { get; set; } = new();

        public List<CreateSolicitudComidaDto> Comidas { get; set; } = new();
    }
}
