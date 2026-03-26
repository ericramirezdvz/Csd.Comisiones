using Csd.Comisiones.Domain.Common;
using Csd.Comisiones.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Domain.Entities
{
    public class SolicitudHotel : AuditableEntity
    {
        public int SolicitudHotelId { get; private set; }

        public int SolicitudEmpleadoId { get; private set; }

        public int? ProveedorId { get; private set; }

        public int TipoHabitacionId { get; private set; }

        public DateTime FechaInicio { get; private set; }

        public DateTime FechaFin { get; private set; }

        public decimal PrecioUnitario { get; private set; }

        public int EstatusDetalleId { get; private set; }

        public SolicitudEmpleado SolicitudEmpleado { get; private set; } = null!;

        public EstatusDetalle EstatusDetalle { get; private set; } = null!;

        public Proveedor Proveedor { get; set; } = null!;

        private SolicitudHotel() { }

        public SolicitudHotel(
            int? hotelId,
            int tipoHabitacionId,
            DateTime fechaInicio,
            DateTime fechaFin,
            decimal precioUnitario)
        {
            if (fechaFin < fechaInicio)
                throw new ArgumentException("La fecha fin no puede ser menor a la fecha inicio.");

            ProveedorId = hotelId;
            TipoHabitacionId = tipoHabitacionId;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            PrecioUnitario = precioUnitario;

            EstatusDetalleId = 1; // Activo
        }
        public void AsignarProveedor(int proveedorId)
        {
            if (EstatusDetalleId == (int)EstatusDetalleEnum.Cancelada)
                throw new InvalidOperationException("No se puede asignar proveedor a un servicio cancelado");

            ProveedorId = proveedorId;
        }

        public void Cancelar()
        {
            EstatusDetalleId = 2; // Cancelado
        }
    }
}
