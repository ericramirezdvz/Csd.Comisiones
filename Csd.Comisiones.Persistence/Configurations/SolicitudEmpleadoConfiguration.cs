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

            // 🔹 Empleado ahora es opcional
            builder.Property(x => x.EmpleadoId)
                .IsRequired(false);

            // 🔹 Nuevo campo: NombreExterno
            builder.Property(x => x.NombreExterno)
                .HasMaxLength(200)
                .IsRequired(false);

            // Nuevo campo: EsExterno
            builder.Property(x => x.EsExterno)
                .IsRequired();

            builder.Property(x => x.FechaInicio)
                .IsRequired();

            builder.Property(x => x.FechaFin)
                .IsRequired();

            builder.Property(x => x.TipoPago)
                .HasConversion<int>()
                .IsRequired(false);

            // Relación con Solicitud
            builder
                .HasOne(x => x.Solicitud)
                .WithMany(x => x.Empleados)
                .HasForeignKey(x => x.SolicitudId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación con Empleado (ahora opcional)
            builder
                .HasOne(x => x.Empleado)
                .WithMany()
                .HasForeignKey(x => x.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
