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
    public class EmpleadoConfiguration : IEntityTypeConfiguration<Empleado>
    {
        public void Configure(EntityTypeBuilder<Empleado> builder)
        {
            builder.ToTable("Empleado");

            builder.HasKey(e => e.EmpleadoId);

            builder.Property(e => e.EmpleadoId)
                .ValueGeneratedOnAdd();

            builder.Property(e => e.NumeroEmpleado)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.NombreCompleto)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Correo)
                .HasMaxLength(150);

            builder.Property(e => e.AreaId)
                .IsRequired();

            builder.Property(e => e.Activo)
                .IsRequired();

            // Índice único para número de empleado
            builder.HasIndex(e => e.NumeroEmpleado)
                .IsUnique();

            // Índice para consultas por área
            builder.HasIndex(e => e.AreaId);
        }
    }
}
