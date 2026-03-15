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
    public class ProveedorConfiguration : IEntityTypeConfiguration<Proveedor>
    {
        public void Configure(EntityTypeBuilder<Proveedor> builder)
        {
            builder.ToTable("Proveedor");

            builder.HasKey(p => p.ProveedorId);

            builder.Property(p => p.ProveedorId)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.TipoProveedor)
                .IsRequired();

            builder.Property(p => p.CiudadId)
                .IsRequired();

            builder.Property(p => p.ProporcionaHospedaje)
                .IsRequired();

            builder.Property(p => p.ProporcionaAlimentos)
                .IsRequired();

            builder.Property(p => p.Activo)
                .IsRequired();

            builder.HasOne<Ciudad>()
                .WithMany()
                .HasForeignKey(p => p.CiudadId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.CiudadId);

            builder.HasIndex(p => new { p.Nombre, p.CiudadId })
                .IsUnique();
        }
    }
}
