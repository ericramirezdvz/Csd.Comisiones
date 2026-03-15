using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.TiposComida.GetTipoComida
{
    public class GetTipoComidaQuery : IRequest<List<GetTipoComidaDto>>
    {
    }
}
