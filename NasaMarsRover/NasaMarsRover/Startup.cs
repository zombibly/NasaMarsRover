using Business.Models;
using Data.Context;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nasa.Rover.Queries;
using NasaMarsRover.Apis;
using RestEase;
using System;
using System.Collections.Generic;

namespace NasaMarsRover
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //These strings can be pulled from an app/web config
            var restClient = RestClient.For<INasaMarsRoversApi>("https://api.nasa.gov/mars-photos/api/v1");
            restClient.ApiKey = "9j9hlxpmmgUieTq2YXeBgE2alZyYZKedpt4gA4kh";

            services.AddSingleton(x => restClient);
            services.AddMediatR(typeof(GetPhotos).Assembly);

            services.AddDbContext<NasaContext>(options => options.UseInMemoryDatabase(databaseName: "Nasa"));
        }

        public static void InitializeDb(NasaContext context)
        {
            //This could probably be initialized using the /manifest/roverName endpoint, but I couldn't get a request to the endpoint to work :(
            var rovers = new List<Rover>
            {
                new Rover
                {
                    Id = 5,
                    Name = "Curiosity",
                    LandingDate = new DateTime(2012, 8, 6),
                    LaunchDate = new DateTime(2011, 11, 26),
                    Status = "Active",
                    MaxSol = 2540,
                    MaxDate = new DateTime(2019, 9, 28),
                    TotalPhotos = 366206
                },
                new Rover
                {
                    Id = 6,
                    Name = "Opportunity",
                    LandingDate = new DateTime(2004, 1, 25),
                    LaunchDate = new DateTime(2003, 7, 7),
                    Status = "complete",
                    MaxSol = 5111,
                    MaxDate = new DateTime(2018, 6, 11),
                    TotalPhotos = 198439
                },
                new Rover
                {
                    Id = 7,
                    Name = "Spirit",
                    LandingDate = new DateTime(2004, 1, 4),
                    LaunchDate = new DateTime(2003, 6, 10),
                    Status = "complete",
                    MaxSol = 2208,
                    MaxDate = new DateTime(2010, 3, 21),
                    TotalPhotos = 124550
                }
            };

            var cameras = new List<Camera>
            {
                new Camera
                {
                    Id = 1,
                    Name = "FHAZ",
                    FullName = "Front Hazard Avoidance Camera",
                    RoverId = 5
                },
                new Camera
                {
                    Id = 2,
                    Name = "NAVCAM",
                    FullName = "Navigation Camera",
                    RoverId = 5
                },
                new Camera
                {
                    Id = 3,
                    Name = "MAST",
                    FullName = "Mast Camera",
                    RoverId = 5
                },
                new Camera
                {
                    Id = 4,
                    Name = "CHEMCAM",
                    FullName = "Chemistry and Camera Complex",
                    RoverId = 5
                },
                new Camera
                {
                    Id = 5,
                    Name = "MAHLI",
                    FullName = "Mars Hand Lens Imager",
                    RoverId = 5
                },
                new Camera
                {
                    Id = 6,
                    Name = "MARDI",
                    FullName = "Mars Descent Imager",
                    RoverId = 5
                },
                new Camera
                {
                    Id = 7,
                    Name = "RHAZ",
                    FullName = "Rear Hazard Avoidance Camera",
                    RoverId = 5
                },
                new Camera
                {
                    Id = 8,
                    Name = "FHAZ",
                    FullName = "Front Hazard Avoidance Camera",
                    RoverId = 6
                },
                new Camera
                {
                    Id = 9,
                    Name = "NAVCAM",
                    FullName = "Navigation Camera",
                    RoverId = 6
                },
                new Camera
                {
                    Id = 10,
                    Name = "PANCAM",
                    FullName = "Panoramic Camera",
                    RoverId = 6
                },
                new Camera
                {
                    Id = 11,
                    Name = "MINITES",
                    FullName = "Miniature Thermal Emission Spectrometer (Mini-TES)",
                    RoverId = 6
                },
                new Camera
                {
                    Id = 12,
                    Name = "ENTRY",
                    FullName = "Entry, Descent, and Landing Camera",
                    RoverId = 6
                },
                new Camera
                {
                    Id = 13,
                    Name = "RHAZ",
                    FullName = "Rear Hazard Avoidance Camera",
                    RoverId = 6
                },
                new Camera
                {
                    Id = 14,
                    Name = "FHAZ",
                    FullName = "Front Hazard Avoidance Camera",
                    RoverId = 7
                },
                new Camera
                {
                    Id = 15,
                    Name = "NAVCAM",
                    FullName = "Navigation Camera",
                    RoverId = 7
                },
                new Camera
                {
                    Id = 16,
                    Name = "PANCAM",
                    FullName = "Panoramic Camera",
                    RoverId = 7
                },
                new Camera
                {
                    Id = 17,
                    Name = "MINITES",
                    FullName = "Miniature Thermal Emission Spectrometer (Mini-TES)",
                    RoverId = 7
                },
                new Camera
                {
                    Id = 18,
                    Name = "ENTRY",
                    FullName = "Entry, Descent, and Landing Camera",
                    RoverId = 7
                },
                new Camera
                {
                    Id = 19,
                    Name = "RHAZ",
                    FullName = "Rear Hazard Avoidance Camera",
                    RoverId = 7
                }
            };

            context.Rovers.AddRange(rovers);
            context.Cameras.AddRange(cameras);
            context.SaveChanges();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();


            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<NasaContext>();
                InitializeDb(context);
            }
        }
    }
}
