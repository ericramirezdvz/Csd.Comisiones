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
    public class AutorizadorConfiguration : IEntityTypeConfiguration<Autorizador>
    {
        public void Configure(EntityTypeBuilder<Autorizador> builder)
        {
            builder.ToTable("Autorizador");

            builder.HasKey(x => x.AutorizadorId);

            builder.Property(x => x.AutorizadorId)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ObraId)
                .IsRequired();

            builder.Property(x => x.EmpleadoId)
                .IsRequired();

            builder.Property(x => x.Nivel)
                .IsRequired();

            builder.Property(x => x.EsAlterno)
                .IsRequired();

            builder.Property(x => x.Activo)
                .IsRequired();

            builder.HasOne<Empleado>()
                .WithMany()
                .HasForeignKey(x => x.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Obra>()
                .WithMany()
                .HasForeignKey(x => x.ObraId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
