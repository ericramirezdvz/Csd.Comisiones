using Csd.Comisiones.Application.Features.TiposComida.GetTipoComida;
using Csd.Comisiones.Application.Features.TiposHabitacion.GetTipoHabitacion;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Csd.Comisiones.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TiposComidaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TiposComidaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var result = await _mediator.Send(new GetTipoComidaQuery());

            return Ok(result);
        }
    }
}
