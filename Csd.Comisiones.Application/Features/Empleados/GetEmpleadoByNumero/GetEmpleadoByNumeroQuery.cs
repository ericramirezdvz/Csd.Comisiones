using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Empleados.GetEmpleadoByNumero
{
    public class GetEmpleadoByNumeroQuery : IRequest<EmpleadoDto>
    {
        public string NumeroEmpleado { get; set; }

        public GetEmpleadoByNumeroQuery(string _numeroEmpleado)
        {
            NumeroEmpleado = _numeroEmpleado;
        }
    }
}
