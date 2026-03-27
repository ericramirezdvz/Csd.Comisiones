using Csd.Comisiones.Application.Features.Obras.GetObras;
using Csd.Comisiones.Application.Features.TiposHabitacion.GetTipoHabitacion;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Csd.Comisiones.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TiposHabitacionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TiposHabitacionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var result = await _mediator.Send(new GetTipoHabitacionQuery());

            return Ok(result);
        }
    }
}
