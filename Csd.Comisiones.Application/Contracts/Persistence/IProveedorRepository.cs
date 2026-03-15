using Csd.Comisiones.Domain.Entities;
using Csd.Comisiones.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Contracts.Persistence
{
    public interface IProveedorRepository : IRepository<Proveedor>
    {
        //Task<IEnumerable<Proveedor>> GetByCiudadAsync(string ciudad);

        //Task<IEnumerable<Proveedor>> GetByTipoAsync(TipoProveedorEnum tipo);
    }
}
