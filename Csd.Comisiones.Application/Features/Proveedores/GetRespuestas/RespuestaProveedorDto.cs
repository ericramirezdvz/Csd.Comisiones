namespace Csd.Comisiones.Application.Features.Proveedores.GetRespuestas
{
    public class RespuestaProveedorDto
    {
        public int RespuestaProveedorId { get; set; }
        public int ProveedorId { get; set; }
        public string ProveedorNombre { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; }
        public DateTime? FechaRespuesta { get; set; }
        public bool? Aceptado { get; set; }
        public string? MotivoRechazo { get; set; }
        public bool Vigente { get; set; }
    }
}
