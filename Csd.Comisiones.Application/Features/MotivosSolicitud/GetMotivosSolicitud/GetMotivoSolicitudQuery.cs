using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.MotivosSolicitud.GetMotivosSolicitud
{
    public class GetMotivoSolicitudQuery : IRequest<List<GetMotivoSolicitudDto>>
    {
    }
}
