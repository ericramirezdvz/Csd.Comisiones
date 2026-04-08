using Csd.Comisiones.Application.Features.UbicacionesAlimento.GetUbicacionAlimento;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Csd.Comisiones.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UbicacionesAlimento : ControllerBase
    {
        private readonly IMediator _mediator;

        public UbicacionesAlimento(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var result = await _mediator.Send(new GetUbicacionAlimentoQuery());
            return Ok(result);
        }
    }
}
