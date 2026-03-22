using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Empleados.GetEmpleadoById
{
    public class GetEmpleadoByIdQuery : IRequest<EmpleadoDto>
    {
        public int EmpleadoId { get; set; }

        public GetEmpleadoByIdQuery(int empleadoId)
        {
            EmpleadoId = empleadoId;
        }
    }
}
