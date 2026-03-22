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
        public int ObraId { get; private set; }
        public int SolicitanteId { get; private set; }
        public int EstatusSolicitudId { get; private set; }
        public DateTime FechaInicio { get; private set; }
        public DateTime FechaFin { get; private set; }
        public string? Comentarios { get; private set; }

        public Area Area { get; private set; } = null!;
        public Obra Obra { get; private set; } = null!;
        public EstatusSolicitud Estatus { get; private set; } = null!;

        private readonly List<SolicitudEmpleado> _empleados = new();
        public IReadOnlyCollection<SolicitudEmpleado> Empleados => _empleados.AsReadOnly();

        private readonly List<SolicitudSeguimiento> _seguimientos = new();
        public IReadOnlyCollection<SolicitudSeguimiento> Seguimientos
            => _seguimientos.AsReadOnly();
        public ICollection<SolicitudAutorizacion> Autorizaciones { get; private set; }
        = new List<SolicitudAutorizacion>();

        private Solicitud() { }

        public Solicitud(
            string folio,
            int areaId,
            int obraId,
            int solicitanteId,
            DateTime fechaInicio,
            DateTime fechaFin,
            string? comentarios)
        {
            if (fechaFin < fechaInicio)
                throw new ArgumentException("La fecha fin no puede ser menor a la fecha inicio.");

            Folio = folio;
            AreaId = areaId;
            ObraId = obraId;
            SolicitanteId = solicitanteId;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            Comentarios = comentarios;

            EstatusSolicitudId = (int)EstatusSolicitudEnum.Borrador;

            var seguimiento = new SolicitudSeguimiento(
                EstatusSolicitudEnum.Borrador,
                Comentarios
                );

            _seguimientos.Add(seguimiento);
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

            RegistrarSeguimiento(nuevoEstatus);
        }

        public void ActualizarComentarios(string? comentarios)
        {
            Comentarios = comentarios;
        }

        private void RegistrarSeguimiento(EstatusSolicitudEnum estatus)
        {
            var seguimiento = new SolicitudSeguimiento(
                estatus,
                Comentarios);

            _seguimientos.Add(seguimiento);
        }

        public void Actualizar(
            int areaId,
            int obraId,
            DateTime fechaInicio,
            DateTime fechaFin,
            string? comentarios)
        {
            if (EstatusSolicitudId != (int)EstatusSolicitudEnum.Borrador)
                throw new InvalidOperationException("Solo se puede editar en estatus Borrador");

            if (fechaFin < fechaInicio)
                throw new ArgumentException("La fecha fin no puede ser menor a la fecha inicio.");

            AreaId = areaId;
            ObraId = obraId;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            Comentarios = comentarios;
        }

        public void LimpiarEmpleados()
        {
            if (EstatusSolicitudId != (int)EstatusSolicitudEnum.Borrador)
                throw new InvalidOperationException("Solo se puede editar en estatus Borrador");

            _empleados.Clear();
        }
    }
}
