using Csd.Comisiones.Domain.Common;
using Csd.Comisiones.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Domain.Entities
{
    public class SolicitudAutorizacion : AuditableEntity
    {
        public int SolicitudAutorizacionId { get; private set; }
        public int SolicitudId { get; private set; }
        public Solicitud Solicitud { get; private set; } = null!;
        public int AutorizadorId { get; private set; }
        public Autorizador Autorizador { get; private set; } = null!;
        public int EstatusAutorizacionId { get; private set; } // Pendiente, Aprobado, Rechazado
        public DateTime? FechaRespuesta { get; private set; }
        public string? Comentarios { get; private set; }

        private SolicitudAutorizacion() { }

        public SolicitudAutorizacion(int solicitudId, int autorizadorId)
        {
            SolicitudId = solicitudId;
            AutorizadorId = autorizadorId;
            EstatusAutorizacionId = (int)EstatusAutorizacionEnum.Pendiente;
        }

        public void AprobarResponsableObra(string? comentarios)
        {
            EstatusAutorizacionId = (int)EstatusAutorizacionEnum.Aprobado;
            FechaRespuesta = DateTime.UtcNow;
            Comentarios = comentarios;
        }

        public void Rechazar(string? comentarios)
        {
            EstatusAutorizacionId = (int)EstatusAutorizacionEnum.Rechazado;
            FechaRespuesta = DateTime.UtcNow;
            Comentarios = comentarios;
        }
    }
}
