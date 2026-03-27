using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.TrackingSolicitud
{
    public class SolicitudSeguimientoDto
    {
        public int Id { get; set; }
        public int EstatusId { get; set; }
        public string Estatus { get; set; } = string.Empty;
        public string? Comentarios { get; set; }
        public DateTime Fecha { get; set; }
    }
}
