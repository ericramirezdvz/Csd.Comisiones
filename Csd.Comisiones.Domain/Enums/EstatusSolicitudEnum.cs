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
        EnProceso = 4, // Pedro (asignación de proveedores) -> Enviar a proveedores -> Proveedores acepta o rechazan -> 
        Terminada = 5,
        Rechazada = 6,
        Cancelada = 7
    }
}
