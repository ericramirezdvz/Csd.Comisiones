using Csd.Comisiones.Domain.Entities;

namespace Csd.Comisiones.Application.Contracts.Infrastructure
{
    public interface ISolpedExcelService
    {
        byte[] GenerarExcelSolped(Solicitud solicitud);
    }
}
