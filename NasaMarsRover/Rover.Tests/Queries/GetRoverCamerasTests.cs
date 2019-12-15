using AutoFixture;
using Data.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Nasa.Rover.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Rover.Tests.Queries
{
    public class GetRoverCamerasTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public async Task ShouldReturnEmptyListIfNoCamerasFound()
        {
            var expectedResult = new GetRoverCameras.Result
            {
                Cameras = new List<GetRoverCameras.Camera>()
            };

            var options = new DbContextOptionsBuilder<NasaContext>()
                   .UseInMemoryDatabase(databaseName: "Nasa")
                   .Options;
            using (var context = new NasaContext(options))
            {
                var query = _fixture.Create<GetRoverCameras.Query>();
                var handler = new GetRoverCameras.Handler(context);

                var result = await handler.Handle(query, CancellationToken.None);

                result.Should().BeEquivalentTo(expectedResult);
            }
        }

        [Fact]
        public async Task ShouldReturnCameras()
        {
            var options = new DbContextOptionsBuilder<NasaContext>()
                   .UseInMemoryDatabase(databaseName: "Nasa")
                   .Options;
            using (var context = new NasaContext(options))
            {
                var rover = _fixture.Create<Business.Models.Rover>();
                var cameras = _fixture.Build<Business.Models.Camera>()
                    .Without(x => x.Rover)
                    .With(x => x.RoverId, rover.Id)
                    .CreateMany()
                    .ToList();

                context.Rovers.Add(rover);
                context.Cameras.AddRange(cameras);
                context.SaveChanges();

                var expectedResult = _fixture.Build<GetRoverCameras.Result>()
                    .With(x => x.Cameras, cameras.Select(x =>
                        new GetRoverCameras.Camera
                        {
                            Name = x.Name,
                            FullName = x.FullName
                        }).ToList()
                    ).Create();


                var query = _fixture.Build<GetRoverCameras.Query>()
                    .With(x => x.RoverId, rover.Id)
                    .Create();
                var handler = new GetRoverCameras.Handler(context);

                var result = await handler.Handle(query, CancellationToken.None);

                result.Should().BeEquivalentTo(expectedResult);
            }
        }

        [Fact]
        public async Task ShouldReturnEmptyListIfInvalidRoverId()
        {
            var options = new DbContextOptionsBuilder<NasaContext>()
                   .UseInMemoryDatabase(databaseName: "Nasa")
                   .Options;
            using (var context = new NasaContext(options))
            {
                var rover = _fixture.Create<Business.Models.Rover>();
                var cameras = _fixture.Build<Business.Models.Camera>()
                    .Without(x => x.Rover)
                    .With(x => x.RoverId, rover.Id)
                    .CreateMany()
                    .ToList();

                context.Rovers.Add(rover);
                context.Cameras.AddRange(cameras);
                context.SaveChanges();

                var expectedResult = _fixture.Build<GetRoverCameras.Result>()
                    .With(x => x.Cameras, new List<GetRoverCameras.Camera>())
                    .Create();

                var query = _fixture.Create<GetRoverCameras.Query>();

                while(query.RoverId == rover.Id)
                {
                    //I doubt this will ever be hit 1 in 2.147b chance, but there's a chance.
                    query = _fixture.Create<GetRoverCameras.Query>();
                }

                var handler = new GetRoverCameras.Handler(context);

                var result = await handler.Handle(query, CancellationToken.None);

                result.Should().BeEquivalentTo(expectedResult);
            }
        }
    }
}
