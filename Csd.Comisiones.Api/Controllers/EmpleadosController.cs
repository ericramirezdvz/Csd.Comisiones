using Csd.Comisiones.Application.Features.Empleados.GetEmpleadoById;
using Csd.Comisiones.Application.Features.Empleados.GetEmpleadoByNumero;
using Csd.Comisiones.Application.Features.Empleados.GetEmpleados;
using Csd.Comisiones.Application.Features.Empleados.ImportEmpleados;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Csd.Comisiones.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmpleadosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]GetEmpleadosQuery query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import(IFormFile file)
        {
            using var stream = file.OpenReadStream();

            var result = await _mediator.Send(new ImportEmpleadosCommand
            {
                FileStream = stream,
                FileName = file.FileName
            });

            return Ok(result);
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetEmpleadoByIdQuery(id));

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("getbynumero/{numero}")]
        public async Task<IActionResult> GetByNumero(string numero)
        {
            var result = await _mediator.Send(new GetEmpleadoByNumeroQuery(numero));

            if (result == null)
                return NotFound();

            return Ok(result);
        }


    }
}
