using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Nasa.Rover.Queries;
using System.Threading;
using System.Threading.Tasks;

namespace NasaMarsRover.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RoversController : Controller
    {
        private readonly IMediator _mediator;

        public RoversController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<GetAllRovers.Result>> GetAllRovers(CancellationToken cancellationToken)
        {
            var query = new GetAllRovers.Query();
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{RoverId}/cameras")]
        public async Task<ActionResult<GetRoverCameras.Result>> GetRoverCameras([FromRoute] GetRoverCameras.Query query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}/photos")]
        public async Task<ActionResult<GetPhotos.Result>> GetPhotos([FromRoute] int id, [FromQuery] GetPhotos.Query query, CancellationToken cancellationToken)
        {
            query.RoverId = id;
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}