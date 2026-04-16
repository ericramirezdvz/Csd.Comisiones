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
            List<ProveedorDetalleDto> detalles,
            Guid token);

        Task SendProveedorRechazoNotificacionAsync(
            string folio,
            string proveedorNombre,
            string motivo);
    }
}
