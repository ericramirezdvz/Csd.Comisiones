using Csd.Comisiones.Domain.Entities;
using Csd.Comisiones.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Persistence.Seeds
{
    public static class EstatusDetalleSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EstatusDetalle>().HasData(
                EstatusDetalle.Seed((int)EstatusDetalleEnum.Borrador, "Borrador", "Solicitud en edición"),                
                EstatusDetalle.Seed((int)EstatusDetalleEnum.EnProceso, "En Proceso", "Solicitud en proceso de los participantes"),
                EstatusDetalle.Seed((int)EstatusDetalleEnum.Aprobada, "Aprobada", "Solicitud autorizada"),
                EstatusDetalle.Seed((int)EstatusDetalleEnum.Rechazada, "Rechazada", "Solicitud rechazada"),
                EstatusDetalle.Seed((int)EstatusDetalleEnum.Cancelada, "Cancelada", "Solicitud cancelada")
            );
        }
    }
}
