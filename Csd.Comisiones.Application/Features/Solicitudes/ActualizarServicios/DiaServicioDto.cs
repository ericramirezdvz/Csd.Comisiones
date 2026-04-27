using System;

namespace Csd.Comisiones.Application.Features.Solicitudes.ActualizarServicios
{
    public class DiaServicioDto
    {
        public DateTime Fecha { get; set; }
        public bool Hospedaje { get; set; }
        public bool Desayuno { get; set; }
        public bool Comida { get; set; }
        public bool Cena { get; set; }
    }
}
