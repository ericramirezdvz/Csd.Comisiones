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
    public class TipoHabitacionConfiguration : IEntityTypeConfiguration<TipoHabitacion>
    {
        public void Configure(EntityTypeBuilder<TipoHabitacion> builder)
        {
            builder.ToTable("TipsHabitacion");

            builder.HasKey(t => t.TipoHabitacionId);

            builder.Property(t => t.TipoHabitacionId)
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
