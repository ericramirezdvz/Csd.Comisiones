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
    namespace TuProyecto.Persistence.Configurations
    {
        public class ObraConfiguration : IEntityTypeConfiguration<Obra>
        {
            public void Configure(EntityTypeBuilder<Obra> builder)
            {
                builder.ToTable("Obra");

                builder.HasKey(c => c.ObraId);

                builder.Property(c => c.ObraId)
                    .ValueGeneratedOnAdd();

                builder.Property(c => c.Nombre)
                    .IsRequired()
                    .HasMaxLength(150);

                builder.Property(c => c.Activo)
                    .IsRequired();

                builder.HasIndex(c => c.Nombre)
                    .IsUnique();
            }
        }
    }
}
