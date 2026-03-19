using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.SendSolicitud
{
    public class EnviarSolicitudCommand : IRequest<Unit>
    {
        public int SolicitudId { get; set; }
    }
}
