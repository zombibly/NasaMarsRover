using AutoFixture;
using Business.Apis.Models;
using Data.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Nasa.Rover.Queries;
using NasaMarsRover.Apis;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Rover.Tests.Queries
{
    public class GetPhotosTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly INasaMarsRoversApi _api = Substitute.For<INasaMarsRoversApi>();

        [Fact]
        public async Task ShouldThrowIfInvalidRoverId()
        {
            var expectedMessage = "Invalid rover Id";
            var options = new DbContextOptionsBuilder<NasaContext>()
                   .UseInMemoryDatabase(databaseName: "Nasa")
                   .Options;
            using(var context = new NasaContext(options))
            {
                var query = _fixture.Create<GetPhotos.Query>();
                var handler = new GetPhotos.Handler(_api, context);

                Action action = () => handler.Handle(query, CancellationToken.None).Wait();
                action.Should().Throw<Exception>().WithMessage(expectedMessage);

                await _api.DidNotReceive().GetPhotos(Arg.Any<string>(), Arg.Any<DateTime>());
            }
        }

        [Fact]
        public async Task ShouldRetrievePhotosFromNasaApiNoSpecifiedCamera()
        {
            var options = new DbContextOptionsBuilder<NasaContext>()
                   .UseInMemoryDatabase(databaseName: "Nasa")
                   .Options;
            using (var context = new NasaContext(options))
            {
                var rover = _fixture.Create<Business.Models.Rover>();
                var camera = _fixture.Build<Business.Models.Camera>()
                    .With(x => x.Rover, rover)
                    .With(x => x.RoverId, rover.Id)
                    .Create();

                context.Rovers.Add(rover);
                context.Cameras.Add(camera);
                context.SaveChanges();

                var query = _fixture.Build<GetPhotos.Query>()
                    .With(x => x.RoverId, rover.Id)
                    .Without(x => x.Camera)
                    .Create();

                var expectedPhotos = _fixture.Build<Business.Models.Photo>()
                    .With(x => x.Camera, camera)
                    .With(x => x.EarthDate, query.Date.Date)
                    .CreateMany()
                    .ToList();

                var expectedResult = new GetPhotos.Result
                {
                    Photos = expectedPhotos.Select(x => new GetPhotos.Photo { Id = x.Id, ImgSrc = x.ImgSrc }).ToList()
                };

                _api.GetPhotos(Arg.Is<string>(x => x == rover.Name), Arg.Is<DateTime>(x => x == query.Date.Date))
                    .Returns(new PhotoResponse
                    {
                        Photos = expectedPhotos
                    });

                var handler = new GetPhotos.Handler(_api, context);
                var result = await handler.Handle(query, CancellationToken.None);

                //Check Result
                result.Should().BeEquivalentTo(expectedResult);

                //Check db to make sure they were inserted properly
                context.Photos.Should().NotBeEmpty();
                foreach(var photo in expectedPhotos)
                {
                    context.Photos.Should().Contain(photo);
                }

                //Make sure the api was actually hit
                await _api.Received().GetPhotos(Arg.Is<string>(x => x == rover.Name), Arg.Is<DateTime>(x => x == query.Date.Date));
            }
        }

        [Fact]
        public async Task ShouldReturnSavedPhotosInsteadOfCallingNasaApi()
        {
            var options = new DbContextOptionsBuilder<NasaContext>()
                   .UseInMemoryDatabase(databaseName: "Nasa")
                   .Options;
            using (var context = new NasaContext(options))
            {
                var date = DateTime.Today.AddDays(-1);
                var rover = _fixture.Create<Business.Models.Rover>();
                var camera = _fixture.Build<Business.Models.Camera>()
                    .With(x => x.Rover, rover)
                    .With(x => x.RoverId, rover.Id)
                    .Create();
                var photos = _fixture.Build<Business.Models.Photo>()
                    .With(x => x.Camera, camera)
                    .With(x => x.CameraId, camera.Id)
                    .With(x => x.EarthDate, date)
                    .CreateMany()
                    .ToList();

                context.Rovers.Add(rover);
                context.Cameras.Add(camera);
                context.Photos.AddRange(photos);
                context.SaveChanges();

                var expectedResult = new GetPhotos.Result
                {
                    Photos = photos.Select(x => new GetPhotos.Photo { Id = x.Id, ImgSrc = x.ImgSrc }).ToList()
                };

                var query = _fixture.Build<GetPhotos.Query>()
                    .With(x => x.RoverId, rover.Id)
                    .With(x => x.Date, date)
                    .Without(x => x.Camera)
                    .Create();

                var handler = new GetPhotos.Handler(_api, context);
                var result = await handler.Handle(query, CancellationToken.None);

                //Check Result
                result.Should().BeEquivalentTo(expectedResult);

                //Make sure api wasn't hit
                await _api.DidNotReceive().GetPhotos(Arg.Any<string>(), Arg.Any<DateTime>());
            }
        }

        [Fact]
        public async Task ShouldFilterPhotosByCamera()
        {
            var options = new DbContextOptionsBuilder<NasaContext>()
                   .UseInMemoryDatabase(databaseName: "Nasa")
                   .Options;
            using (var context = new NasaContext(options))
            {
                var date = DateTime.Today.AddDays(-1);
                var rover = _fixture.Create<Business.Models.Rover>();
                var camera1 = _fixture.Build<Business.Models.Camera>()
                    .With(x => x.Rover, rover)
                    .With(x => x.RoverId, rover.Id)
                    .Create();
                var camera2 = _fixture.Build<Business.Models.Camera>()
                    .With(x => x.Rover, rover)
                    .With(x => x.RoverId, rover.Id)
                    .Create();
                var expectedPhotos = _fixture.Build<Business.Models.Photo>()
                    .With(x => x.Camera, camera1)
                    .With(x => x.CameraId, camera1.Id)
                    .With(x => x.EarthDate, date)
                    .CreateMany()
                    .ToList();
                var otherPhotos = _fixture.Build<Business.Models.Photo>()
                    .With(x => x.Camera, camera2)
                    .With(x => x.CameraId, camera2.Id)
                    .With(x => x.EarthDate, date)
                    .CreateMany()
                    .ToList();

                context.Rovers.Add(rover);
                context.Cameras.Add(camera1);
                context.Cameras.Add(camera2);
                context.Photos.AddRange(expectedPhotos);
                context.Photos.AddRange(otherPhotos);
                context.SaveChanges();

                var expectedResult = new GetPhotos.Result
                {
                    Photos = expectedPhotos.Select(x => new GetPhotos.Photo { Id = x.Id, ImgSrc = x.ImgSrc }).ToList()
                };

                var query = _fixture.Build<GetPhotos.Query>()
                    .With(x => x.RoverId, rover.Id)
                    .With(x => x.Date, date)
                    .With(x => x.Camera, camera1.Name)
                    .Create();

                var handler = new GetPhotos.Handler(_api, context);
                var result = await handler.Handle(query, CancellationToken.None);

                //Check Result
                result.Should().BeEquivalentTo(expectedResult);

                //Make sure api wasn't hit
                await _api.DidNotReceive().GetPhotos(Arg.Any<string>(), Arg.Any<DateTime>());
            }
        }
    }
}
