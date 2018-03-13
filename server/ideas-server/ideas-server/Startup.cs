using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoE.Ideas.Core;
using CoE.Ideas.Shared.Extensions;
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
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // configure application specific logging
            services.AddSingleton<Serilog.ILogger>(x => new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "Initiatives")
                .Enrich.WithProperty("Module", "Server")
                .ReadFrom.Configuration(Configuration)
                .CreateLogger());

            services.AddLocalInitiativeConfiguration(Configuration.GetConnectionString("IdeaDatabase"));
            services.AddWordPressSecurity(Configuration.GetConnectionString("WordPressDatabase"), 
                Configuration.GetSection("WordPress"));

            services.AddInitiativeMessaging(Configuration.GetConnectionString("IdeaServiceBus"),
                Configuration["Ideas:ServiceBusTopic"]);

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
