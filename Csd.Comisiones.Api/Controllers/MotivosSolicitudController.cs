using Csd.Comisiones.Application.Features.MotivosSolicitud.GetMotivosSolicitud;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Csd.Comisiones.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MotivosSolicitudController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MotivosSolicitudController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var result = await _mediator.Send(new GetMotivoSolicitudQuery());
            return Ok(result);
        }
    }
}
