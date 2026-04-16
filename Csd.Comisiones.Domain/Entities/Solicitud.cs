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
        public int? CiudadId { get; set; }
        public int? MotivoSolicitudId { get; set; }
        public int SolicitanteId { get; private set; }
        public int EstatusSolicitudId { get; private set; }
        public DateTime FechaInicio { get; private set; }
        public DateTime FechaFin { get; private set; }
        public string? Comentarios { get; private set; }

        public Area Area { get; private set; } = null!;
        public Obra Obra { get; private set; } = null!;
        public Ciudad Ciudad { get; private set; } = null!;
        public MotivoSolicitud MotivoSolicitud { get; private set; } = null!;
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
            int ciudadId,
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
            CiudadId = ciudadId;
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

        private void CambiarEstatus(EstatusSolicitudEnum nuevoEstatus, string? comentarioSeguimiento = null)
        {
            EstatusSolicitudId = (int)nuevoEstatus;
            RegistrarSeguimiento(nuevoEstatus, comentarioSeguimiento);
        }

        public void ActualizarComentarios(string? comentarios)
        {
            Comentarios = comentarios;
        }

        private void RegistrarSeguimiento(EstatusSolicitudEnum estatus, string? comentario = null)
        {
            var seguimiento = new SolicitudSeguimiento(
                estatus,
                comentario);
            _seguimientos.Add(seguimiento);
        }

        public void Actualizar(
            int areaId,
            int obraId,
            int ciudadId,
            DateTime fechaInicio,
            DateTime fechaFin,
            string? comentarios)
        {
            var estatusPermitidos = new[]
            {
                (int)EstatusSolicitudEnum.Borrador,
                (int)EstatusSolicitudEnum.AutorizadaPorResponsableDeObra,
                (int)EstatusSolicitudEnum.EnProceso
            };

            if (!estatusPermitidos.Contains(EstatusSolicitudId))
                throw new InvalidOperationException("Solo se puede editar en estatus Borrador, Autorizada o En Proceso");

            if (fechaFin < fechaInicio)
                throw new ArgumentException("La fecha fin no puede ser menor a la fecha inicio.");

            AreaId = areaId;
            ObraId = obraId;
            CiudadId = ciudadId;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            Comentarios = comentarios;
        }

        public void LimpiarEmpleados()
        {
            var estatusPermitidos = new[]
            {
                (int)EstatusSolicitudEnum.Borrador,
                (int)EstatusSolicitudEnum.AutorizadaPorResponsableDeObra,
                (int)EstatusSolicitudEnum.EnProceso
            };

            if (!estatusPermitidos.Contains(EstatusSolicitudId))
                throw new InvalidOperationException("Solo se puede editar en estatus Borrador, Autorizada o En Proceso");

            _empleados.Clear();
        }

        public void Enviar()
        {
            if (EstatusSolicitudId != (int)EstatusSolicitudEnum.Borrador)
                throw new InvalidOperationException("Solo se puede enviar desde Borrador");

            if (!_empleados.Any())
                throw new InvalidOperationException("Debe existir al menos un empleado");

            CambiarEstatus(EstatusSolicitudEnum.EnAutorizaciónPorResponsableDeObra, "Solicitud enviada a autorización");
        }

        public void Aprobar()
        {
            if (EstatusSolicitudId != (int)EstatusSolicitudEnum.EnAutorizaciónPorResponsableDeObra)
                throw new InvalidOperationException("No se puede aprobar en este estado");

            CambiarEstatus(EstatusSolicitudEnum.AutorizadaPorResponsableDeObra, "Solicitud autorizada por responsable de obra");
            CambiarEstatus(EstatusSolicitudEnum.EnProceso, "Solicitud en proceso — pendiente de asignación de bloques");
        }

        public void Rechazar(int autorizadorId, string? comentario)
        {
            if (EstatusSolicitudId != (int)EstatusSolicitudEnum.EnAutorizaciónPorResponsableDeObra)
                throw new InvalidOperationException("No se puede rechazar en este estado");

            var autorizacion = Autorizaciones
                .FirstOrDefault(x => x.AutorizadorId == autorizadorId);

            if (autorizacion == null)
                throw new InvalidOperationException("No tienes autorización para esta solicitud");

            if (autorizacion.EstatusAutorizacionId != (int)EstatusAutorizacionEnum.Pendiente)
                throw new InvalidOperationException("Esta autorización ya fue atendida");

            autorizacion.Rechazar(comentario);

            Comentarios = comentario;
            CambiarEstatus(EstatusSolicitudEnum.Rechazada, comentario);
        }

        public void Terminar()
        {
            if (EstatusSolicitudId != (int)EstatusSolicitudEnum.EnProceso)
                throw new InvalidOperationException("Solo se puede terminar en proceso");

            if (!_empleados.Any())
                throw new InvalidOperationException("La solicitud no tiene empleados");

            foreach (var empleado in _empleados)
            {
                var tieneServicios =
                    empleado.Hoteles.Any() ||
                    empleado.Comidas.Any();

                if (!tieneServicios)
                    throw new InvalidOperationException(
                        $"El empleado {empleado.EmpleadoId} no tiene servicios asignados");
            }

            CambiarEstatus(EstatusSolicitudEnum.Terminada, "Checkout completado — Solped generada");
        }

        public void Cancelar(string? comentario)
        {
            if (EstatusSolicitudId == (int)EstatusSolicitudEnum.Terminada)
                throw new InvalidOperationException("No se puede cancelar una solicitud terminada");

            if (EstatusSolicitudId == (int)EstatusSolicitudEnum.Cancelada)
                throw new InvalidOperationException("La solicitud ya está cancelada");

            Comentarios = comentario;
            CambiarEstatus(EstatusSolicitudEnum.Cancelada, comentario ?? "Solicitud cancelada");
        }
    }
}
