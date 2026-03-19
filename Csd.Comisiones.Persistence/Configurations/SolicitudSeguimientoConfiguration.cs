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
    public class SolicitudSeguimientoConfiguration
        : IEntityTypeConfiguration<SolicitudSeguimiento>
    {
        public void Configure(EntityTypeBuilder<SolicitudSeguimiento> builder)
        {
            builder.ToTable("SolicitudSeguimiento");

            builder.HasKey(x => x.SolicitudSeguimientoId);

            builder.Property(x => x.Comentarios)
                .HasMaxLength(500);

            builder.Property(x => x.Fecha)
                .IsRequired();

            builder.Property(x => x.EstatusSolicitudId)
                .IsRequired();

            builder.HasOne(x => x.Solicitud)
                .WithMany(x => x.Seguimientos)
                .HasForeignKey(x => x.SolicitudId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Estatus)
                .WithMany()
                .HasForeignKey(x => x.EstatusSolicitudId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
