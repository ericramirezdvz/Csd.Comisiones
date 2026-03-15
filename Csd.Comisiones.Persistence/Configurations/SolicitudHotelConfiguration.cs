using Csd.Comisiones.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Persistence.Configurations
{
    public class SolicitudHotelConfiguration : IEntityTypeConfiguration<SolicitudHotel>
    {
        public void Configure(EntityTypeBuilder<SolicitudHotel> builder)
        {
            builder.ToTable("SolicitudHotel");

            builder.HasKey(x => x.SolicitudHotelId);

            builder.Property(x => x.FechaInicio)
                .IsRequired();

            builder.Property(x => x.FechaFin)
                .IsRequired();

            builder.Property(x => x.PrecioUnitario)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            // Relación con SolicitudEmpleado
            builder
                .HasOne(x => x.SolicitudEmpleado)
                .WithMany(x => x.Hoteles)
                .HasForeignKey(x => x.SolicitudEmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Proveedor)
                .WithMany()
                .HasForeignKey(x => x.ProveedorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación con EstatusDetalle
            builder
                .HasOne(x => x.EstatusDetalle)
                .WithMany()
                .HasForeignKey(x => x.EstatusDetalleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación con TipoHabitacion
            builder
                .HasOne<TipoHabitacion>()
                .WithMany()
                .HasForeignKey(x => x.TipoHabitacionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
