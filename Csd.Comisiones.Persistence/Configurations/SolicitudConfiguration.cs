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
    public class SolicitudConfiguration : IEntityTypeConfiguration<Solicitud>
    {
        public void Configure(EntityTypeBuilder<Solicitud> builder)
        {
            builder.ToTable("Solicitud");

            builder.HasKey(x => x.SolicitudId);

            builder.Property(x => x.Folio)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.AreaId)
                .IsRequired();

            builder.Property(x => x.SolicitanteId)
                .IsRequired();

            builder.Property(x => x.FechaInicio)
                .IsRequired();

            builder.Property(x => x.FechaFin)
                .IsRequired();

            builder.Property(x => x.EstatusSolicitudId)
                .IsRequired();

            builder.Property(x => x.Comentarios)
                .HasMaxLength(500);

            // RELACIÓN CON AREA
            builder
                .HasOne(x => x.Area)
                .WithMany()
                .HasForeignKey(x => x.AreaId)
                .OnDelete(DeleteBehavior.Restrict);

            // RELACIÓN CON OBRA
            builder
                .HasOne(x => x.Obra)
                .WithMany()
                .HasForeignKey(x => x.ObraId)
                .OnDelete(DeleteBehavior.Restrict);

            // RELACIÓN CON ESTATUS
            builder
                .HasOne(x => x.Estatus)
                .WithMany()
                .HasForeignKey(x => x.EstatusSolicitudId)
                .OnDelete(DeleteBehavior.Restrict);

            // RELACIÓN CON EMPLEADOS
            builder
                .HasMany(x => x.Empleados)
                .WithOne(x => x.Solicitud)
                .HasForeignKey(x => x.SolicitudId);

            // IMPORTANTE para colecciones encapsuladas
            builder
                .Navigation(x => x.Empleados)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
