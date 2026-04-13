using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Proveedores.SendProveedores
{
    public record EnviarProveedoresCommand(int SolicitudId) : IRequest<Unit>;
}
