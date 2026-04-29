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
        public int AreaId { get; set; }
        public string? Correo { get; set; }
        public int TipoAsignacion { get; set; } // 1=Servicios, 2=Pago
        public decimal? MontoPago { get; set; }
        public int? TipoPago { get; set; } // 1=14x14 Costa afuera, 2=30x0 Tierra
        public bool EsExterno { get; set; }
        public string? NombreExterno { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public List<HotelSolicitudDto> Hoteles { get; set; } = new();
        public List<ComidaSolicitudDto> Comidas { get; set; } = new();
    }
}
