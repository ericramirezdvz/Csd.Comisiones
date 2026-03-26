using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Common.Models
{
    public class EmpleadoEmailDto
    {
        public string Nombre { get; set; } = "";
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool RequiereHotel { get; set; }
        public bool Desayuno { get; set; }
        public bool Almuerzo { get; set; }
        public bool Cena { get; set; }
    }
}
