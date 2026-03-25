using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.CompleteSolicitud
{
    public class CompletarSolicitudCommand : IRequest<Unit>
    {
        public int SolicitudId { get; set; }
    }
}
