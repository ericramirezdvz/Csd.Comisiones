using Csd.Comisiones.Application.Contracts.Persistence;
using Csd.Comisiones.Domain.Common;
using Csd.Comisiones.Domain.Entities;
using Csd.Comisiones.Persistence.Seeds;
using Microsoft.EntityFrameworkCore;

namespace Csd.Comisiones.Persistence
{
    public class ComisionesDbContext : DbContext, IApplicationDbContext
    {
        public ComisionesDbContext(DbContextOptions<ComisionesDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<Ciudad> Ciudad { get; set; }
        public DbSet<Empleado> Empleado { get; set; }
        public DbSet<EstatusSolicitud> EstatusSolicitud { get; set; }
        public DbSet<EstatusDetalle> EstatusDetalle { get; set; }
        public DbSet<Proveedor> Proveedor { get; set; }
        public DbSet<ProveedorServicio> ProveedorServicio { get; set; }
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
        public DbSet<UbicacionAlimento> UbicacionAlimento { get; set; }
        public DbSet<RespuestaProveedor> RespuestaProveedor { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ComisionesDbContext).Assembly);

            //EstatusSolicitudSeed.Seed(modelBuilder);
            //EstatusDetalleSeed.Seed(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var entity = modelBuilder.Entity(entityType.ClrType);

                    entity.Property(nameof(AuditableEntity.FechaCreacion))
                        .HasColumnType("datetime")
                        .IsRequired();

                    entity.Property(nameof(AuditableEntity.CreadoPor))
                        .HasMaxLength(100)
                        .IsRequired();

                    entity.Property(nameof(AuditableEntity.FechaModificacion))
                        .HasColumnType("datetime");

                    entity.Property(nameof(AuditableEntity.ModificadoPor))
                        .HasMaxLength(100);
                }
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries<AuditableEntity>()
                .Where(e => e.State == EntityState.Added);

            foreach (var entry in entries)
            {
                entry.Entity.FechaCreacion = DateTime.UtcNow;
                entry.Entity.CreadoPor = "SYSTEM";
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
