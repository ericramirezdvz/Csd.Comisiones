using Csd.Comisiones.Api.Dtos;
using Csd.Comisiones.Application.Features.Solicitudes.ApproveSolicitud;
using Csd.Comisiones.Application.Features.Solicitudes.CancelSolicitud;
using Csd.Comisiones.Application.Features.Solicitudes.CompleteSolicitud;
using Csd.Comisiones.Application.Features.Solicitudes.CreateSolicitud;
using Csd.Comisiones.Application.Features.Solicitudes.GetSolicitudById;
using Csd.Comisiones.Application.Features.Solicitudes.GetSolicitudes;
using Csd.Comisiones.Application.Features.Solicitudes.RejectSolicitud;
using Csd.Comisiones.Application.Features.Solicitudes.SendSolicitud;
using Csd.Comisiones.Application.Features.Solicitudes.TrackingSolicitud;
using Csd.Comisiones.Application.Features.Solicitudes.UpdateSolicitud;
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

        [HttpPost("{id}/send")]
        public async Task<IActionResult> Send(int id)
        {
            await _mediator.Send(new EnviarSolicitudCommand
            {
                SolicitudId = id
            });

            return NoContent();
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id, [FromBody]AprobarSolicitudRequest request)
        {
            await _mediator.Send(new AprobarSolicitudCommand
            {
                SolicitudId = id,
                Comentarios = request.Comentarios
            });

            return NoContent();
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id, [FromBody] CancelarSolicitudRequest request)
        {
            await _mediator.Send(new CancelarSolicitudCommand
            {
                SolicitudId = id,
                Comentarios = request.Comentarios
            });

            return NoContent();
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            await _mediator.Send(new CompletarSolicitudCommand
            {
                SolicitudId = id
            });

            return NoContent();
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(int id, [FromBody] RechazarSolicitudCommand request)
        {
            await _mediator.Send(new RechazarSolicitudCommand
            {
                SolicitudId = id,
                Comentarios = request.Comentarios,
                AutorizadorId = request.AutorizadorId
            });

            return NoContent();
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSolicitudCommand request)
        {
            await _mediator.Send(new UpdateSolicitudCommand
            {
                SolicitudId = id
            });

            return NoContent();
        }

        [HttpGet("{id}/tracking")]
        public async Task<IActionResult> Tracking(int id)
        {
            var result = await _mediator.Send(new SeguimientoSolicitudQuery(id));
            if (result == null)
                return NotFound();

            return Ok(result);
        }

    }
}
