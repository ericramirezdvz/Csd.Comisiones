using Csd.Comisiones.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Domain.Entities
{
    public class EstatusDetalle : AuditableEntity
    {
        public int EstatusDetalleId { get; private set; }
        public string Nombre { get; private set; } = string.Empty;
        public string Descripcion { get; private set; } = string.Empty;
        public bool Activo { get; private set; }

        private EstatusDetalle() { }

        public EstatusDetalle(
            string nombre,
            string descripcion)
        {
            Nombre = nombre;
            Descripcion = descripcion;
            Activo = true;
        }

        private EstatusDetalle(
        int id,
        string nombre,
        string descripcion,
        bool activo)
        {
            EstatusDetalleId = id;
            Nombre = nombre;
            Descripcion = descripcion;
            Activo = activo;
            FechaCreacion = DateTime.UtcNow;
            CreadoPor = "System";
        }

        public static EstatusDetalle Seed(
        int id,
        string nombre,
        string descripcion)
        {
            return new EstatusDetalle(id, nombre, descripcion, true);
        }
    }
}
