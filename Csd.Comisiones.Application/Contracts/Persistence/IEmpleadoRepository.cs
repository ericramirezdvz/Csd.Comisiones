using Csd.Comisiones.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Contracts.Persistence
{
    public interface IEmpleadoRepository : IRepository<Empleado>
    {
        Task<Empleado?> GetByEmailAsync(string email);
    }
}
