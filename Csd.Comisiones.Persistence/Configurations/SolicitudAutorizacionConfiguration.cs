using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csd.Comisiones.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Csd.Comisiones.Persistence.Configurations
{
    public class SolicitudAutorizacionConfiguration : IEntityTypeConfiguration<SolicitudAutorizacion>
    {
        public void Configure(EntityTypeBuilder<SolicitudAutorizacion> builder)
        {
            builder.ToTable("SolicitudAutorizacion");

            builder.HasKey(x => x.SolicitudAutorizacionId);

            builder.Property(x => x.SolicitudAutorizacionId)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.SolicitudId)
                .IsRequired();

            builder.Property(x => x.AutorizadorId)
                .IsRequired();

            builder.Property(x => x.EstatusAutorizacionId)
                .IsRequired();

            builder.Property(x => x.FechaRespuesta)
                .IsRequired(false);

            builder.Property(x => x.Comentarios)
                .HasMaxLength(500);

            // Relación con Solicitud
            builder.HasOne(x => x.Solicitud)
                .WithMany(x => x.Autorizaciones)
                .HasForeignKey(x => x.SolicitudId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación con Autorizador
            builder.HasOne(x => x.Autorizador)
                .WithMany(x => x.Autorizaciones)
                .HasForeignKey(x => x.AutorizadorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
