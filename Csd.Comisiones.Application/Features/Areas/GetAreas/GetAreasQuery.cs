using MediatR;
using System.Collections.Generic;

namespace Csd.Comisiones.Application.Features.Areas.GetAreas
{
    public class GetAreasQuery : IRequest<List<AreaDto>>
    {
    }
}
