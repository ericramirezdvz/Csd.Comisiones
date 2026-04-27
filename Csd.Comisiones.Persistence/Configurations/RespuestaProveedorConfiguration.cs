using Csd.Comisiones.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Csd.Comisiones.Persistence.Configurations
{
    public class RespuestaProveedorConfiguration : IEntityTypeConfiguration<RespuestaProveedor>
    {
        public void Configure(EntityTypeBuilder<RespuestaProveedor> builder)
        {
            builder.ToTable("RespuestaProveedor");

            builder.HasKey(x => x.RespuestaProveedorId);

            builder.Property(x => x.Token)
                .IsRequired();

            builder.HasIndex(x => x.Token)
                .IsUnique();

            builder.Property(x => x.FechaEnvio)
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(x => x.FechaRespuesta)
                .HasColumnType("datetime");

            builder.Property(x => x.Aceptado);

            builder.Property(x => x.MotivoRechazo)
                .HasMaxLength(1000);

            builder.Property(x => x.Vigente)
                .IsRequired()
                .HasDefaultValue(true);

            builder
                .HasOne(x => x.Solicitud)
                .WithMany()
                .HasForeignKey(x => x.SolicitudId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(x => x.Proveedor)
                .WithMany()
                .HasForeignKey(x => x.ProveedorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
