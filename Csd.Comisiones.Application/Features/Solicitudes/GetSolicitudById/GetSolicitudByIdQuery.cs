using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.GetSolicitudById
{
    public class GetSolicitudByIdQuery : IRequest<SolicitudDetalleDto>
    {
        public int SolicitudId { get; set; }

        public GetSolicitudByIdQuery(int solicitudId)
        {
            SolicitudId = solicitudId;
        }
    }
}
