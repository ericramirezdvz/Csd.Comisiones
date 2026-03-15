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
    public class EstatusDetalleConfiguration : IEntityTypeConfiguration<EstatusDetalle>
    {
        public void Configure(EntityTypeBuilder<EstatusDetalle> builder)
        {
            builder.ToTable("EstatusDetalle");

            builder.HasKey(e => e.EstatusDetalleId);

            builder.Property(e => e.EstatusDetalleId)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Descripcion)
                .HasMaxLength(250);

            builder.Property(e => e.Activo)
                .IsRequired();

            builder.HasIndex(e => e.Nombre)
                .IsUnique();
        }
    }
}
