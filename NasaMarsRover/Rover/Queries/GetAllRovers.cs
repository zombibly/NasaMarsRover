using Data.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nasa.Rover.Queries
{
    public class GetAllRovers
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result
        {
            public List<Rover> Rovers { get; set; }
        }

        public class Rover
        {
            public int Id { get; set; }
            public string Name { get; set; }
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
                var rovers = await _context.Rovers.Select(x => new Rover
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync(cancellationToken);

                return new Result
                {
                    Rovers = rovers
                };
            }
        }
    }
}
