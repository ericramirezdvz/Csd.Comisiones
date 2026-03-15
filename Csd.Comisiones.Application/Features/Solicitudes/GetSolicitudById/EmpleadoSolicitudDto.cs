using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.GetSolicitudById
{
    public class EmpleadoSolicitudDto
    {
        public int SolicitudEmpleadoId { get; set; }
        public int EmpleadoId { get; set; }
        public string NumeroEmpleado { get; set; }
        public string NombreCompleto { get; set; }

        public List<HotelSolicitudDto> Hoteles { get; set; } = new();
        public List<ComidaSolicitudDto> Comidas { get; set; } = new();
    }
}
