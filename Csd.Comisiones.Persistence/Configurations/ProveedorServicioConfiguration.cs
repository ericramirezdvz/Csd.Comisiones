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
    public class ProveedorServicioConfiguration : IEntityTypeConfiguration<ProveedorServicio>
    {
        public void Configure(EntityTypeBuilder<ProveedorServicio> builder)
        {
            builder.ToTable("ProveedorServicio");

            builder.HasKey(x => x.ProveedorServicioId);

            builder.Property(x => x.ProveedorServicioId)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ProveedorId)
                .IsRequired();

            builder.Property(x => x.TipoServicio)
                .IsRequired();

            builder.Property(x => x.Precio)
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            // Relación con Proveedor
            builder.HasOne(x => x.Proveedor)
                .WithMany(p => p.Servicios) // o .WithMany(p => p.Servicios) si agregas la navegación
                .HasForeignKey(x => x.ProveedorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
