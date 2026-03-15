using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Domain.Common
{
    public class AuditableEntity
    {
        public DateTime FechaCreacion { get; set; }
        public string CreadoPor { get; set; } = string.Empty;
        public DateTime? FechaModificacion { get; set; }
        public string? ModificadoPor { get; set; }
    }
}
