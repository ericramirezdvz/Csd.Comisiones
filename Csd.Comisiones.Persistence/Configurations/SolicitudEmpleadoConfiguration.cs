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
    public class SolicitudEmpleadoConfiguration : IEntityTypeConfiguration<SolicitudEmpleado>
    {
        public void Configure(EntityTypeBuilder<SolicitudEmpleado> builder)
        {
            builder.ToTable("SolicitudEmpleado");

            builder.HasKey(x => x.SolicitudEmpleadoId);

            builder.Property(x => x.EmpleadoId)
                .IsRequired();

            builder.Property(x => x.FechaInicio)
                .IsRequired();

            builder.Property(x => x.FechaFin)
                .IsRequired();

            // Relación con Solicitud Empleado
            builder
                .HasOne(x => x.Solicitud)
                .WithMany(x => x.Empleados)
                .HasForeignKey(x => x.SolicitudId);

            // Relación con Empleado
            builder
                .HasOne(x => x.Empleado)
                .WithMany()
                .HasForeignKey(x => x.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
