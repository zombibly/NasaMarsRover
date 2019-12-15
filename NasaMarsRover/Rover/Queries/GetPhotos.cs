using Data.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NasaMarsRover.Apis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nasa.Rover.Queries
{
    public class GetPhotos
    {
        public class Query : IRequest<Result>
        {
            public int RoverId { get; set; }
            public DateTime Date { get; set; }
            public string Camera { get; set; }
        }

        public class Result
        {
            public List<Photo> Photos { get; set; }
        }

        public class Photo
        {
            public int Id { get; set; }
            public string ImgSrc { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result>
        {
            private readonly INasaMarsRoversApi _api;
            private readonly NasaContext _context;

            public Handler(INasaMarsRoversApi api, NasaContext context)
            {
                _api = api;
                _context = context;
            }

            public async Task<Result> Handle(Query query, CancellationToken cancellationToken)
            {
                //Check to see if the photos have already been gathered (will fail if it's today)
                var gathered = false;
                if(query.Date.Date != DateTime.Today)
                {
                    gathered = _context.Photos.Any(x => x.EarthDate == query.Date.Date &&
                        x.Camera.Rover.Id == query.RoverId);
                }

                if(!gathered)
                {
                    var rover = _context.Rovers.FirstOrDefault(x => x.Id == query.RoverId);

                    if(rover == null)
                    {
                        throw new Exception("Invalid rover Id");
                    }

                    //Call Nasa Api to get photos
                    List<Business.Models.Photo> gatheredPhotos;
                    try
                    {
                        var result = await _api.GetPhotos(rover.Name, query.Date.Date);
                        gatheredPhotos = result.Photos;
                    }
                    catch(Exception e)
                    {
                        throw e;
                    }

                    foreach(var photo in gatheredPhotos)
                    {
                        photo.Camera = _context.Cameras.FirstOrDefault(x => x.RoverId == rover.Id && x.Name == photo.Camera.Name);
                        photo.CameraId = photo.Camera.Id;
                    }

                    //Save photos to db
                    _context.Photos.AddRange(gatheredPhotos);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                //filter photos based on information sent from client
                var photos = await _context.Photos.Where(x => 
                    x.EarthDate == query.Date.Date &&
                    x.Camera.Rover.Id == query.RoverId &&
                    (query.Camera == null || x.Camera.Name == query.Camera))
                    .Select(x => new Photo { Id = x.Id, ImgSrc = x.ImgSrc })
                    .ToListAsync(cancellationToken);


                //return photos
                return new Result
                {
                    Photos = photos
                };
            }
        }
    }
}
