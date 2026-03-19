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
    public static class EstatusSolicitudSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EstatusSolicitud>().HasData(
                EstatusSolicitud.Seed((int)EstatusSolicitudEnum.Borrador, "Borrador", "Solicitud en edición"),                
                EstatusSolicitud.Seed((int)EstatusSolicitudEnum.EnAutorizaciónPorResponsableDeObra, "En Autorización por Responsable de Obra", "Solicitud enviada al responsable de obra"),
                EstatusSolicitud.Seed((int)EstatusSolicitudEnum.AutorizadaPorResposableDeObra, "Autorizada por Resposable de Obra", "Solicitud autorizada por el responsable de obra"),                
                EstatusSolicitud.Seed((int)EstatusSolicitudEnum.EnAutorizacionPorPermisos, "En Autorización por Permisos", "Solicitud en autorización por permisos"),
                EstatusSolicitud.Seed((int)EstatusSolicitudEnum.AutorizadaPorPermisos, "Autorizada por Permisos", "Solicitud autorizada por permisos"),
                EstatusSolicitud.Seed((int)EstatusSolicitudEnum.Asignada, "Asignada", "Solicitud asignada para seguimiento"),
                EstatusSolicitud.Seed((int)EstatusSolicitudEnum.EnProceso, "En Proceso", "Solicitud en proceso de atención"),
                EstatusSolicitud.Seed((int)EstatusSolicitudEnum.Terminada, "Terminada", "Solicitud terminada"),
                EstatusSolicitud.Seed((int)EstatusSolicitudEnum.Rechazada, "Rechazada", "Solicitud rechazada"),
                EstatusSolicitud.Seed((int)EstatusSolicitudEnum.Cancelada, "Cancelada", "Solicitud cancelada")
            );
        }
    }
}
