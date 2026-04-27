using Csd.Comisiones.Application.Common.Models;
using Csd.Comisiones.Application.Features.Proveedores.SendProveedores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Contracts.Infrastructure
{
    public interface IEmailService
    {
        Task SendSolicitudPendienteAsync(
            int solicitudId,
            int autorizadorId,
            string correo,
            string folio,
            string obra,
            DateTime fechaInicio,
            DateTime fechaFin,
            List<EmpleadoEmailDto> empleados);

        Task SendSolicitudAprobadaAsync(
            string correo,
            string folio,
            string obra,
            DateTime fechaInicio,
            DateTime fechaFin,
            List<EmpleadoEmailDto> empleados);

        Task SendSolicitudRechazadaAsync(
            string correo,
            string folio,
            string obra,
            DateTime fechaInicio,
            DateTime fechaFin,
            List<EmpleadoEmailDto> empleados,
            string? motivo);

        Task SendSolicitudProveedorAsync(
            string correo,
            string proveedorNombre,
            string folio,
            string subFolio,
            List<ProveedorDetalleDto> detalles,
            Guid token,
            bool esConciliacion = false);

        Task SendProveedorRechazoNotificacionAsync(
            string folio,
            string proveedorNombre,
            string motivo);

        Task SendSolpedAsync(
            string folio,
            string obra,
            string area,
            string periodo,
            string tablaAlimentacion,
            string tablaHospedaje,
            byte[]? excelAdjunto = null);

        /// <summary>
        /// Envía el "Generador de Servicios y Subcontratos" (Excel) a un proveedor.
        /// </summary>
        Task SendGeneradorProveedorAsync(
            string correoProveedor,
            string proveedorNombre,
            string folio,
            byte[] excelAdjunto);

        Task SendSolicitudProveedorModificadaAsync(
            string correo,
            string proveedorNombre,
            string folio,
            string empleadoNombre,
            List<ProveedorDetalleDto> eliminados,
            List<ProveedorDetalleDto> vigentes,
            Guid token);
    }
}
