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
        public class MotivoSolicitudConfiguration : IEntityTypeConfiguration<MotivoSolicitud>
        {
            public void Configure(EntityTypeBuilder<MotivoSolicitud> builder)
            {
                builder.ToTable("MotivoSolicitud");

                builder.HasKey(c => c.MotivoSolicitudId);

                builder.Property(c => c.MotivoSolicitudId)
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
