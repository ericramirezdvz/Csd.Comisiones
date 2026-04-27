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
        public DbSet<EstatusSolicitud> EstatusSolicitud { get; set; }
        public DbSet<EstatusDetalle> EstatusDetalle { get; set; }
        public DbSet<Proveedor> Proveedor { get; set; }
        public DbSet<Solicitud> Solicitud { get; set; }
        public DbSet<SolicitudComida> SolicitudComida { get; set; }
        public DbSet<SolicitudEmpleado> SolicitudEmpleado { get; set; }
        public DbSet<SolicitudHotel> SolicitudHotel { get; set; }
        public DbSet<TipoComida> TipoComida { get; set; }
        public DbSet<TipoHabitacion> TipoHabitacion { get; set; }
        public DbSet<SolicitudSeguimiento> SolicitudSeguimiento { get; set; }
        public DbSet<Obra> Obra { get; set; }
        public DbSet<Area> Area { get; set; }
        public DbSet<Autorizador> Autorizador { get; set; }
        public DbSet<SolicitudAutorizacion> SolicitudAutorizacion { get; set; }
        public DbSet<ProveedorServicio> ProveedorServicio { get; set; }
        public DbSet<UbicacionAlimento> UbicacionAlimento { get; set; }
        public DbSet<RespuestaProveedor> RespuestaProveedor { get; set; }
        public DbSet<MotivoSolicitud> MotivoSolicitud { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<UsuarioRol> UsuariRol { get; set; }
        public DbSet<Rol> Rol { get; set; }
    }
}
