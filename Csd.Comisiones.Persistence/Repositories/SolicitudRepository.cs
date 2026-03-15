using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Persistence.Repositories
{
    public class SolicitudRepository : Repository<Solicitud>, ISolicitudRepository
    {
        public SolicitudRepository(ComisionesDbContext dbContext) : base(dbContext)
        {
        }
    }
}
