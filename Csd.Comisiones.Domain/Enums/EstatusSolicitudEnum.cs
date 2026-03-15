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
        Enviada = 2,
        EnRevision = 3,
        Autorizada = 4,
        Rechazada = 5,
        Parcial = 6,
        Cancelada = 7
    }
}
