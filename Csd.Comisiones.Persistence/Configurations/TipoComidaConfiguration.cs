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
    public class TipoComidaConfiguration : IEntityTypeConfiguration<TipoComida>
    {
        public void Configure(EntityTypeBuilder<TipoComida> builder)
        {
            builder.ToTable("TipoComida");

            builder.HasKey(t => t.TipoComidaId);

            builder.Property(t => t.TipoComidaId)
                .ValueGeneratedOnAdd();

            builder.Property(t => t.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Activo)
                .IsRequired();

            builder.HasIndex(t => t.Nombre)
                .IsUnique();
        }
    }
}
