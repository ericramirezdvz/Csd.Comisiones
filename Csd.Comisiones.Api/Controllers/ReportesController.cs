using Csd.Comisiones.Application.Features.Reportes.ReporteServicios;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Csd.Comisiones.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("servicios")]
        public async Task<IActionResult> Exportar([FromQuery] DateTime? inicio, [FromQuery] DateTime? fin)
        {
            var file = await _mediator.Send(new ExportReporteServiciosQuery() { FechaInicio = inicio, FechaFin = fin });

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ReporteServicios.xlsx"
            );
        }
    }
}
