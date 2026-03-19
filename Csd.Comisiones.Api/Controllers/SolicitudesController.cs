using Csd.Comisiones.Application.Features.Solicitudes.ApproveSolicitud;
using Csd.Comisiones.Application.Features.Solicitudes.CreateSolicitud;
using Csd.Comisiones.Application.Features.Solicitudes.GetSolicitudById;
using Csd.Comisiones.Application.Features.Solicitudes.GetSolicitudes;
using Csd.Comisiones.Application.Features.Solicitudes.SendSolicitud;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Csd.Comisiones.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SolicitudesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSolicitudCommand command)
        {
            var id = await _mediator.Send(command);

            return CreatedAtAction(nameof(Create), new { id }, id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetSolicitudByIdQuery(id));

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]GetSolicitudesQuery query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            await _mediator.Send(new AprobarSolicitudCommand
            {
                SolicitudId = id
            });

            return NoContent();
        }

        [HttpPost("{id}/send")]
        public async Task<IActionResult> Send(int id)
        {
            await _mediator.Send(new EnviarSolicitudCommand
            {
                SolicitudId = id
            });

            return NoContent();
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Rechazar(int id)
        {
            await _mediator.Send(new EnviarSolicitudCommand
            {
                SolicitudId = id
            });

            return NoContent();
        }
    }
}
