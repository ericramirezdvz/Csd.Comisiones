using Csd.Comisiones.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Domain.Entities
{
    public class Autorizador : AuditableEntity
    {
        public int AutorizadorId { get; private set; }
        public int ObraId { get; private set; }
        public int EmpleadoId { get; private set; }
        public int Nivel { get; private set; }
        public bool EsAlterno { get; private set; }
        public bool Activo { get; private set; }
        public ICollection<SolicitudAutorizacion> Autorizaciones { get; private set; }
        = new List<SolicitudAutorizacion>();

        private Autorizador() { }

        public Autorizador(
            int obraId,
            int empleadoId,
            int nivel,
            bool esAlterno)
        {
            ObraId = obraId;
            EmpleadoId = empleadoId;
            Nivel = nivel;
            EsAlterno = esAlterno;
            Activo = true;
        }

        public void Desactivar()
        {
            Activo = false;
        }
    }
}
