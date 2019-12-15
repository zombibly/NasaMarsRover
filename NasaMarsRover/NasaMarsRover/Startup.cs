using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Context;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NasaMarsRover.Apis;
using RestEase;
using Rover.Queries;

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
        }
    }
}
