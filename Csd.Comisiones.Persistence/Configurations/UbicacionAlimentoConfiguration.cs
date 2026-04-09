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
    public class UbicacionAlimentoConfiguration : IEntityTypeConfiguration<UbicacionAlimento>
    {
        public void Configure(EntityTypeBuilder<UbicacionAlimento> builder)
        {
            builder.ToTable("UbicacionAlimento");

            builder.HasKey(t => t.UbicacionAlimentoId);

            builder.Property(t => t.UbicacionAlimentoId)
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
