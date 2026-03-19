using Csd.Comisiones.Application.Features.Proveedores.GetProveedores;
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
    }
}
