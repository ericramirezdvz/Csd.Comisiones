using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.CreateSolicitud
{
    public class CreateSolicitudHotelDto
    {
        public int ProveedorId { get; set; }

        public int TipoHabitacionId { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public decimal PrecioUnitario { get; set; }
    }
}
