using Csd.Comisiones.Application.Features.Ciudades.GetCiudad;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Csd.Comisiones.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CiudadesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CiudadesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetCiudades()
        {
            var result = await _mediator.Send(new GetCiudadQuery());
            return Ok(result);
        }
    }
}
