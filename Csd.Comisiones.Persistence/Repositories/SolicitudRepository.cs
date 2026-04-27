using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Domain.Entities;
using Microsoft.EntityFrameworkCore;
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

        public new async Task<Solicitud?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(s => s.Empleados)
                    .ThenInclude(e => e.Hoteles)
                .Include(s => s.Empleados)
                    .ThenInclude(e => e.Comidas)
                .FirstOrDefaultAsync(s => s.SolicitudId == id);
        }
    }
}
