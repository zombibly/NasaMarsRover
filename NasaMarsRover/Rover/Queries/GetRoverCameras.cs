using Data.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nasa.Rover.Queries
{
    public class GetRoverCameras
    {
        public class Query : IRequest<Result>
        {
            public int RoverId { get; set; }
        }

        public class Result
        {
            public List<Camera> Cameras { get; set; }
        }

        public class Camera
        {
            public string Name { get; set; }
            public string FullName { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly NasaContext _context;
            public Handler(NasaContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(Query query, CancellationToken cancellationToken)
            {
                //Could do some check to see if it's a valid rover first ¯\_(ツ)_/¯
                var cameras = await _context.Cameras.Where(x => x.RoverId == query.RoverId).Select(x => new Camera
                {
                    Name = x.Name,
                    FullName = x.FullName
                }).ToListAsync(cancellationToken);

                return new Result
                {
                    Cameras = cameras
                };
            }
        }
    }
}
