using Csd.Comisiones.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Domain.Entities
{
    public class SolicitudEmpleado : AuditableEntity
    {
        public int SolicitudEmpleadoId { get; private set; }
        public int SolicitudId { get; private set; }
        public int EmpleadoId { get; private set; }

        public DateTime FechaInicio { get; private set; }
        public DateTime FechaFin { get; private set; }
        public Solicitud Solicitud { get; private set; } = null!;
        public Empleado Empleado { get; set; } = null!;

        private readonly List<SolicitudHotel> _hoteles = new();
        public IReadOnlyCollection<SolicitudHotel> Hoteles => _hoteles.AsReadOnly();

        private readonly List<SolicitudComida> _comidas = new();
        public IReadOnlyCollection<SolicitudComida> Comidas => _comidas.AsReadOnly();

        private SolicitudEmpleado() { }

        public SolicitudEmpleado(
        int empleadoId,
        DateTime fechaInicio,
        DateTime fechaFin)
        {
            if (fechaFin < fechaInicio)
                throw new ArgumentException("La fecha fin no puede ser menor a la fecha inicio.");

            EmpleadoId = empleadoId;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
        }

        public void AgregarHotel(SolicitudHotel hotel)
        {
            _hoteles.Add(hotel);
        }

        public void AgregarComida(SolicitudComida comida)
        {
            _comidas.Add(comida);
        }
    }
}
