using Csd.Comisiones.Domain.Common;
using Csd.Comisiones.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Domain.Entities
{
    public class SolicitudSeguimiento : AuditableEntity
    {
        public int SolicitudSeguimientoId { get; private set; }

        public int SolicitudId { get; private set; }

        public int EstatusSolicitudId { get; private set; }

        public string? Comentarios { get; private set; }

        public DateTime Fecha { get; private set; }

        public EstatusSolicitud Estatus { get; private set; } = null!;

        public Solicitud Solicitud { get; private set; } = null!;

        private SolicitudSeguimiento() { }

        public SolicitudSeguimiento(
            EstatusSolicitudEnum estatus,
            string? comentarios)
        {
            EstatusSolicitudId = (int)estatus;
            Comentarios = comentarios;
            Fecha = DateTime.UtcNow;
        }
    }
}
