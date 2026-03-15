using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csd.Comisiones.Application.Common.Models;
using MediatR;

namespace Csd.Comisiones.Application.Features.Proveedores.GetProveedores
{
    public class GetProveedoresQuery : IRequest<PagedResult<ProveedorDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
