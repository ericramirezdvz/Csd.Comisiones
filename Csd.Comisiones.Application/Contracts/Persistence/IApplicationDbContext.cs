using Csd.Comisiones.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Contracts.Persistence
{
    public interface IApplicationDbContext
    {
        public DbSet<Ciudad> Ciudad { get; set; }
        public DbSet<Empleado> Empleado { get; set; }
        public DbSet<Proveedor> Proveedor { get; set; }
        public DbSet<TipoComida> TipoComida { get; set; }
        public DbSet<TipoHabitacion> TipoHabitacion { get; set; }
        public DbSet<EstatusSolicitud> EstatusSolicitud { get; set; }
        public DbSet<Solicitud> Solicitud { get; set; }
    }
}
