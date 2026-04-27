using Csd.Comisiones.Domain.Entities;

namespace Csd.Comisiones.Application.Contracts.Infrastructure
{
    public interface ISolpedExcelService
    {
        byte[] GenerarExcelSolped(Solicitud solicitud);

        /// <summary>
        /// Genera un Excel "Generador de Servicios y Subcontratos" por cada proveedor
        /// con los servicios activos de la solicitud.
        /// Key = ProveedorId, Value = (nombreProveedor, bytes del Excel)
        /// </summary>
        Dictionary<int, (string NombreProveedor, string Correo, byte[] Excel)> GenerarExcelPorProveedor(Solicitud solicitud);
    }
}
