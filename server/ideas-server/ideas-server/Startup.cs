using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoE.Ideas.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace CoE.Ideas.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public IConfiguration Configuration { get; }
        private readonly ILoggerFactory _loggerFactory;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure Serilog logging
            _loggerFactory.AddSerilog();
            services.AddSingleton<Serilog.ILogger>(x =>
                new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .WriteTo.AzureTableStorage("DefaultEndpointsProtocol=https;AccountName=octavia;AccountKey=bFLOflELWg3NQ3OWe8Yk9gTFl03JgC0sM6orZdfMI5E5pKo+OuqNowaIUSD6qA0VDNZuUFa7K8WRIELTu0MHgw==;EndpointSuffix=core.windows.net",
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information, 
                    storageTableName: "InitiativesDevLog")
                .CreateLogger());

            services.AddIdeaConfiguration(
                Configuration.GetConnectionString("IdeaDatabase"), 
                Configuration["Ideas:WordPressUrl"],
                Configuration.GetConnectionString("IdeaServiceBus"),
                Configuration["Ideas:ServiceBusTopic"]);


            services.AddIdeaAuthSecurity(Configuration["Authorization:JwtSecretKey"],
                Configuration["Authorization:CoeAuthKey"], 
                Configuration["Authorization:CoeAuthIV"],
                Configuration["Ideas:WordPressUrl"]);

            services.AddProjectManagementConfiguration(Configuration.GetConnectionString("IdeaProjectManagementDatabase"));

            services.AddMvc();

            services.AddAutoMapper();
        }


        protected void ConfigureCors(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(builder =>
            {
                //(env.IsDevelopment() ? builder.AllowAnyOrigin() : builder.WithOrigins(Configuration["Ideas:WordPressUrl"]))
                //    .WithMethods("OPTIONS", "GET", "PUT", "POST", "DELETE")
                //    .AllowAnyHeader();
                builder.AllowAnyOrigin()
                    .WithMethods("OPTIONS", "GET", "PUT", "POST", "DELETE")
                    .AllowAnyHeader();

            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else if (env.IsProduction())
            {
                //https://docs.microsoft.com/en-us/aspnet/core/publishing/linuxproduction?tabs=aspnetcore2x
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
                });
            }

            ConfigureCors(app, env);

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
