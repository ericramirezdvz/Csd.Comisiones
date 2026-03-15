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
    public class EstatusSolicitudConfiguration : IEntityTypeConfiguration<EstatusSolicitud>
    {
        public void Configure(EntityTypeBuilder<EstatusSolicitud> builder)
        {
            builder.ToTable("EstatusSolicitud");

            builder.HasKey(x => x.EstatusSolicitudId);

            builder.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Activo)
                .IsRequired();
        }
    }
}
