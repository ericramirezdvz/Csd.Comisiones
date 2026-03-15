using Csd.Comisiones.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Domain.Entities
{
    public class Empleado : AuditableEntity
    {
        public int EmpleadoId { get; private set; }

        public string NumeroEmpleado { get; private set; } = string.Empty;
        public string NombreCompleto { get; private set; } = string.Empty;

        public int AreaId { get; private set; }

        public string? Correo { get; private set; }

        public bool Activo { get; private set; }

        private Empleado() { }

        public Empleado(
            string numeroEmpleado,
            string nombreCompleto,
            int areaId,
            string? correo)
        {
            NumeroEmpleado = numeroEmpleado;
            NombreCompleto = nombreCompleto;
            AreaId = areaId;
            Correo = correo;
            Activo = true;
        }

        public void Desactivar()
        {
            Activo = false;
        }
    }
}
