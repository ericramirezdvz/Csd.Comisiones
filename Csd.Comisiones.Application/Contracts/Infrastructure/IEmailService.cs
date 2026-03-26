using Csd.Comisiones.Application.Common.Models;
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
            string correo,
            string folio,
            string obra,
            DateTime fechaInicio,
            DateTime fechaFin,
            List<EmpleadoEmailDto> empleados);
    }
}
