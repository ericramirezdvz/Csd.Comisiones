using Csd.Comisiones.Application.Features.Proveedores.GetProveedores;
using Csd.Comisiones.Application.Features.Proveedores.GetRespuestas;
using Csd.Comisiones.Application.Features.Proveedores.SendProveedores;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Csd.Comisiones.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProveedoresController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetProveedoresQuery query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPost("{solicitudId}/enviar")]
        public async Task<IActionResult> EnviarAProveedores(int solicitudId)
        {
            await _mediator.Send(new EnviarProveedoresCommand(solicitudId));
            return Ok();
        }

        [HttpGet("{solicitudId}/respuestas")]
        public async Task<IActionResult> GetRespuestas(int solicitudId)
        {
            var result = await _mediator.Send(new GetRespuestasProveedorQuery(solicitudId));
            return Ok(result);
        }
    }
}
