using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.TrackingSolicitud
{
    public class SeguimientoSolicitudQuery : IRequest<List<SolicitudSeguimientoDto>>
    {
        public int SolicitudId { get; set; }

        public SeguimientoSolicitudQuery(int solicitudId)
        {
            SolicitudId = solicitudId;
        }
    }
}
