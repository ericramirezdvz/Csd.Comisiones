using Csd.Comisiones.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Contracts.Persistence
{
    public interface ISolicitudRepository : IRepository<Solicitud>
    {
        //Task<IEnumerable<Solicitud>> GetByEmpleadoAsync(int empleadoId);

        //Task<IEnumerable<Solicitud>> GetByEstatusAsync(int estatusId);
    }
}
