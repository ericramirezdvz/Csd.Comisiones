using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Domain.Enums
{
    public enum EstatusSolicitudEnum
    {
        Borrador = 1,
        EnAutorizaciónPorResponsableDeObra = 2,
        AutorizadaPorResposableDeObra = 3,
        EnAutorizacionPorPermisos = 4,
        AutorizadaPorPermisos = 5,
        Asignada = 6,
        EnProceso = 7,
        Terminada = 8,
        Rechazada = 9,
        Cancelada = 10
    }
}
