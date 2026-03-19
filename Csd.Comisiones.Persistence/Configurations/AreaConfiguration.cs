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
        public class AreaConfiguration : IEntityTypeConfiguration<Area>
        {
            public void Configure(EntityTypeBuilder<Area> builder)
            {
                builder.ToTable("Area");

                builder.HasKey(c => c.AreaId);

                builder.Property(c => c.AreaId)
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
