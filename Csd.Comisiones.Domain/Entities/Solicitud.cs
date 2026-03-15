using Csd.Comisiones.Domain.Common;
using Csd.Comisiones.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Domain.Entities
{
    public class Solicitud : AuditableEntity
    {
        public int SolicitudId { get; private set; }
        public string Folio { get; private set; } = string.Empty;

        public int AreaId { get; private set; }
        public int SolicitanteId { get; private set; }

        public int EstatusSolicitudId { get; private set; }

        public DateTime FechaInicio { get; private set; }
        public DateTime FechaFin { get; private set; }

        public EstatusSolicitud Estatus { get; private set; } = null!;

        private readonly List<SolicitudEmpleado> _empleados = new();
        public IReadOnlyCollection<SolicitudEmpleado> Empleados => _empleados.AsReadOnly();

        private Solicitud() { }

        public Solicitud(
            string folio,
            int areaId,
            int solicitanteId,
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            if (fechaFin < fechaInicio)
                throw new ArgumentException("La fecha fin no puede ser menor a la fecha inicio.");

            Folio = folio;
            AreaId = areaId;
            SolicitanteId = solicitanteId;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;

            EstatusSolicitudId = (int)EstatusSolicitudEnum.Borrador;
        }

        public void AgregarEmpleado(SolicitudEmpleado empleado)
        {
            if (_empleados.Any(e => e.EmpleadoId == empleado.EmpleadoId))
                throw new InvalidOperationException("El empleado ya está agregado a la solicitud.");

            _empleados.Add(empleado);
        }

        public void CambiarEstatus(EstatusSolicitudEnum nuevoEstatus)
        {
            EstatusSolicitudId = (int)nuevoEstatus;
        }
    }
}
