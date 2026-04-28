using Csd.Comisiones.Domain.Common;
using Csd.Comisiones.Domain.Enums;
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
        public int? EmpleadoId { get; private set; }
        public string? NombreExterno { get; private set; }
        public bool EsExterno { get; private set; }
        public DateTime FechaInicio { get; private set; }
        public DateTime FechaFin { get; private set; }
        public TipoAsignacionEnum TipoAsignacion { get; private set; }
        public TipoPagoEnum? TipoPago { get; private set; }
        public decimal? MontoPago { get; private set; }
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
            TipoAsignacion = TipoAsignacionEnum.Servicios;
        }

        public void AgregarHotel(SolicitudHotel hotel)
        {
            if (TipoAsignacion == TipoAsignacionEnum.Pago)
                throw new InvalidOperationException("No se pueden agregar hoteles a un pago");

            _hoteles.Add(hotel);
        }

        public void AgregarComida(SolicitudComida comida)
        {
            if (TipoAsignacion == TipoAsignacionEnum.Pago)
                throw new InvalidOperationException("No se pueden agregar comidas a un pago");

            _comidas.Add(comida);
        }

        public static SolicitudEmpleado CrearPago(
        int? empleadoId,
        string? nombreExterno,
        bool esExterno,
        DateTime fechaInicio,
        DateTime fechaFin,
        decimal monto,
        TipoPagoEnum tipoPago)
        {
            if (fechaFin < fechaInicio)
                throw new ArgumentException("La fecha fin no puede ser menor a la fecha inicio.");

            if (esExterno && string.IsNullOrWhiteSpace(nombreExterno))
                throw new ArgumentException("El nombre del externo es requerido");

            if (!esExterno && !empleadoId.HasValue)
                throw new ArgumentException("El empleado es requerido");

            return new SolicitudEmpleado
            {
                EmpleadoId = empleadoId,
                NombreExterno = nombreExterno,
                EsExterno = esExterno,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                TipoAsignacion = TipoAsignacionEnum.Pago,
                MontoPago = monto > 0 ? monto : null,
                TipoPago = tipoPago
            };
        }

        public void ConvertirAPago(decimal monto)
        {
            if (_hoteles.Any() || _comidas.Any())
                throw new InvalidOperationException("No se puede convertir a pago si ya existen servicios");

            if (monto <= 0)
                throw new ArgumentException("El monto debe ser mayor a 0");

            var hoy = DateTime.Now.Date;

            if (FechaInicio.Date < hoy.AddDays(3))
                throw new InvalidOperationException("No se pueden crear solicitudes de pago con menos de 3 días de anticipación");


            TipoAsignacion = TipoAsignacionEnum.Pago;
            MontoPago = monto;
        }

        public void ConvertirAServicios()
        {
            TipoAsignacion = TipoAsignacionEnum.Servicios;
            MontoPago = null;
        }

        public static SolicitudEmpleado CrearInterno(
        int empleadoId,
        DateTime fechaInicio,
        DateTime fechaFin)
        {
            if (empleadoId <= 0)
                throw new ArgumentException("El empleado es requerido");

            return new SolicitudEmpleado
            {
                EmpleadoId = empleadoId,
                EsExterno = false,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };
        }

        public static SolicitudEmpleado CrearExterno(
        string nombreExterno,
        DateTime fechaInicio,
        DateTime fechaFin)
        {
            if (string.IsNullOrWhiteSpace(nombreExterno))
                throw new ArgumentException("El nombre del externo es requerido");

            return new SolicitudEmpleado
            {
                NombreExterno = nombreExterno,
                EsExterno = true,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };
        }


    }
}
