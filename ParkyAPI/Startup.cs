using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ParkyAPI.Data;
using ParkyAPI.ParkyMapper;
using ParkyAPI.Repository;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI
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
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<INationalParkRepository, NationalParkRepository>();
            services.AddScoped<ITrailRepository, TrailRepository>();
            services.AddAutoMapper(typeof(ParkyMappings));

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;

            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("ParkyOpenApiSpec", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "Parky API",
                    Version = "1",
                    Description = "Parky API - Learning How to develope API and consume it",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "Albertopaulo@gmail.com",
                        Name = "Alberto Paulo",
                        Url = new Uri("https://www.linkedin.com/in/albertopaulo/")

                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
                    }

                });

                //    options.SwaggerDoc("ParkyOpenApiSpecTrails", new Microsoft.OpenApi.Models.OpenApiInfo()
                //    {
                //        Title = "Parky API Trails",
                //        Version = "1",
                //        Description = "Parky API Trails - Learning How to develope API and consume it",
                //        Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                //        {
                //            Email = "Albertopaulo@gmail.com",
                //            Name = "Alberto Paulo",
                //            Url = new Uri("https://www.linkedin.com/in/albertopaulo/")

                //        },
                //        License = new Microsoft.OpenApi.Models.OpenApiLicense()
                //        {
                //            Name = "MIT License",
                //            Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
                //        }

                //    });

                var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var cmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
                options.IncludeXmlComments(cmlCommentsFullPath);
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/ParkyOpenApiSpec/swagger.json", "Parky API");
                //options.SwaggerEndpoint("/swagger/ParkyOpenApiSpecTrails/swagger.json", "Parky API Trails");
                options.RoutePrefix = "";
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
