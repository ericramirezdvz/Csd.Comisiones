using Csd.Comisiones.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Domain.Entities
{
    public class EstatusSolicitud : AuditableEntity
    {
        public int EstatusSolicitudId { get; private set; }
        public string Nombre { get; private set; } = string.Empty;
        public string Descripcion { get; private set; } = string.Empty;
        public bool Activo { get; private set; }

        private EstatusSolicitud() { }

        public EstatusSolicitud(
            string nombre,
            string descripcion)
        {
            Nombre = nombre;
            Descripcion = descripcion;
            Activo = true;
        }

        private EstatusSolicitud(
        int id,
        string nombre,
        string descripcion,
        bool activo)
        {
            EstatusSolicitudId = id;
            Nombre = nombre;
            Descripcion = descripcion;
            Activo = activo;
            FechaCreacion = DateTime.UtcNow;
            CreadoPor = "System";
        }

        public static EstatusSolicitud Seed(
        int id,
        string nombre,
        string descripcion)
        {
            return new EstatusSolicitud(id, nombre, descripcion, true);
        }
    }
}
