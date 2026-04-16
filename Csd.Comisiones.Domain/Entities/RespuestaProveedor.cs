using Csd.Comisiones.Domain.Common;
using System;

namespace Csd.Comisiones.Domain.Entities
{
    public class RespuestaProveedor : AuditableEntity
    {
        public int RespuestaProveedorId { get; private set; }
        public int SolicitudId { get; private set; }
        public int ProveedorId { get; private set; }
        public Guid Token { get; private set; }
        public DateTime FechaEnvio { get; private set; }
        public DateTime? FechaRespuesta { get; private set; }
        public bool? Aceptado { get; private set; }
        public string? MotivoRechazo { get; private set; }
        public bool Vigente { get; private set; }

        public Solicitud Solicitud { get; private set; } = null!;
        public Proveedor Proveedor { get; private set; } = null!;

        private RespuestaProveedor() { }

        public RespuestaProveedor(int solicitudId, int proveedorId)
        {
            SolicitudId = solicitudId;
            ProveedorId = proveedorId;
            Token = Guid.NewGuid();
            FechaEnvio = DateTime.UtcNow;
            Vigente = true;
        }

        public void Aceptar()
        {
            ValidarPuedeResponder();
            Aceptado = true;
            FechaRespuesta = DateTime.UtcNow;
            Vigente = false;
        }

        public void Rechazar(string motivo)
        {
            ValidarPuedeResponder();
            Aceptado = false;
            MotivoRechazo = motivo;
            FechaRespuesta = DateTime.UtcNow;
            Vigente = false;
        }

        public void Invalidar()
        {
            Vigente = false;
        }

        private void ValidarPuedeResponder()
        {
            if (!Vigente)
                throw new InvalidOperationException("Este enlace ya no es válido. Es posible que ya se haya respondido o se haya generado uno nuevo.");

            if (FechaRespuesta.HasValue)
                throw new InvalidOperationException("Esta solicitud ya fue respondida anteriormente.");
        }
    }
}
