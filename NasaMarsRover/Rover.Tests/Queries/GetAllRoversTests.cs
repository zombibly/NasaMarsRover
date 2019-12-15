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
    public class GetAllRoversTests
    {
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public async Task ShouldHandleNoRoversFound()
        {
            var expectedResult = new GetAllRovers.Result
            {
                Rovers = new List<GetAllRovers.Rover>()
            };

            var options = new DbContextOptionsBuilder<NasaContext>()
                   .UseInMemoryDatabase(databaseName: "Nasa")
                   .Options;
            using(var context = new NasaContext(options))
            {
                var handler = new GetAllRovers.Handler(context);
                var result = await handler.Handle(new GetAllRovers.Query(), CancellationToken.None);

                result.Should().BeEquivalentTo(expectedResult);
            }
        }

        [Fact]
        public async Task ShouldRetrieveRoverInformation()
        {
            var options = new DbContextOptionsBuilder<NasaContext>()
                   .UseInMemoryDatabase(databaseName: "Nasa")
                   .Options;
            using (var context = new NasaContext(options))
            {
                var rovers = _fixture.CreateMany<Business.Models.Rover>().ToList();
                var expectedResult = new GetAllRovers.Result
                {
                    Rovers = rovers.Select(x => new GetAllRovers.Rover
                    {
                        Id = x.Id,
                        Name = x.Name
                    }).ToList()
                };
                context.Rovers.AddRange(rovers);
                context.SaveChanges();

                var handler = new GetAllRovers.Handler(context);
                var result = await handler.Handle(new GetAllRovers.Query(), CancellationToken.None);

                result.Should().BeEquivalentTo(expectedResult);
            }
        }
    }
}
