using Csd.Comisiones.Domain.Common;
using Csd.Comisiones.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Domain.Entities
{
    public class SolicitudComida : AuditableEntity
    {
        public int SolicitudComidaId { get; private set; }

        public int SolicitudEmpleadoId { get; private set; }

        public int TipoComidaId { get; private set; }

        public int ProveedorId { get; private set; }

        public int UbicacionId { get; private set; }

        public DateTime FechaInicio { get; private set; }

        public DateTime FechaFin { get; private set; }

        public decimal PrecioUnitario { get; private set; }

        public int EstatusDetalleId { get; private set; }

        public SolicitudEmpleado SolicitudEmpleado { get; private set; } = null!;

        public EstatusDetalle EstatusDetalle { get; private set; } = null!;
        public TipoComida TipoComida { get; set; } = null!;
        public Proveedor Proveedor { get; set; } = null!;

        private SolicitudComida() { }

        public SolicitudComida(
            int tipoComidaId,
            int proveedorId,
            int ubicacionId,
            DateTime fechaInicio,
            DateTime fechaFin,
            decimal precioUnitario)
        {
            if (fechaFin < fechaInicio)
                throw new ArgumentException("La fecha fin no puede ser menor a la fecha inicio.");

            TipoComidaId = tipoComidaId;
            ProveedorId = proveedorId;
            UbicacionId = ubicacionId;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            PrecioUnitario = precioUnitario;

            EstatusDetalleId = (int)EstatusDetalleEnum.Borrador; // Activo
        }

        public void Cancelar()
        {
            EstatusDetalleId = (int)EstatusDetalleEnum.Cancelada; // Cancelado
        }
    }
}
