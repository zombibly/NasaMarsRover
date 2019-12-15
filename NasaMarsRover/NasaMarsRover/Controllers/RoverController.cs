using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rover.Queries;
using System.Threading;
using System.Threading.Tasks;

namespace NasaMarsRover.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RoverController : Controller
    {
        private readonly IMediator _mediator;

        public RoverController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("photos")]
        public async Task<ActionResult<GetPhotos.Result>> GetPhotos([FromQuery] GetPhotos.Query query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result;
        }
    }
}