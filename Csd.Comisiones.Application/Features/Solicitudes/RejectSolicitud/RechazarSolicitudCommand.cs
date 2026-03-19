using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.RejectSolicitud
{
    public class RechazarSolicitudCommand : IRequest<Unit>
    {
        public int SolicitudId { get; set; }
        public int AutorizadorId { get; set; }
        public string? Comentarios { get; set; }
    }
}
