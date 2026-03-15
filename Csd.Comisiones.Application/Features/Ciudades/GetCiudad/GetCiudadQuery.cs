using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Ciudades.GetCiudad
{
    public class GetCiudadQuery : IRequest<List<GetCiudadDto>>
    {
    }
}
