using Csd.Comisiones.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Solicitudes.GetSolicitudes
{
    public class GetSolicitudesQuery : IRequest<PagedResult<SolicitudListItemDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Si tiene valor, filtra solo las solicitudes creadas por este UsuarioId.
        /// null = sin filtro (admin ve todo).
        /// </summary>
        public int? SolicitanteId { get; set; }
    }
}
