using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Csd.Comisiones.Application.Features.Solicitudes.ApproveSolicitud
{
    public class AprobarSolicitudCommand : IRequest<bool>
    {
        public int SolicitudId { get; set; }
    }
}
