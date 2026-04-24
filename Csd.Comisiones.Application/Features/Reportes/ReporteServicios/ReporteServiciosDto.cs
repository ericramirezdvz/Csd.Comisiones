using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csd.Comisiones.Application.Features.Reportes.ReporteServicios
{
    public class ReporteServiciosDto
    {
        public string AreaSolicitante { get; set; } = "";
        public DateTime FechaSolicitud { get; set; }
        public string Obra { get; set; } = "";
        public string Motivo { get; set; } = "";

        public string ClaseValoracion { get; set; } = "Servicio";
        public string Material { get; set; } = "";
        public string Descripcion { get; set; } = "";

        public decimal Cantidad { get; set; }
        public string UM { get; set; } = "SER";

        public string Folio { get; set; }

        public DateTime FechaRealServicio { get; set; }

        public decimal MontoSinIVA { get; set; }
        public decimal IVA { get; set; }
        public decimal Monto { get; set; }

        public string Moneda { get; set; } = "MXN";
        public string Proveedor { get; set; } = "";

        public DateTime Fecha { get; set; }
        public string Alcance { get; set; } = "";
    }
}
