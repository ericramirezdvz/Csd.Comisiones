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
    public class SolicitudComidaConfiguration : IEntityTypeConfiguration<SolicitudComida>
    {
        public void Configure(EntityTypeBuilder<SolicitudComida> builder)
        {
            builder.ToTable("SolicitudComida");

            builder.HasKey(x => x.SolicitudComidaId);

            builder.Property(x => x.PrecioUnitario)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(x => x.FechaInicio)
                .IsRequired();

            builder.Property(x => x.FechaFin)
                .IsRequired();

            // Relación con SolicitudEmpleado
            builder
                .HasOne(x => x.SolicitudEmpleado)
                .WithMany(x => x.Comidas)
                .HasForeignKey(x => x.SolicitudEmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.TipoComida)
                .WithMany()
                .HasForeignKey(x => x.TipoComidaId)
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
        }
    }
}
