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

namespace CoE.Ideas.Server
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
            services.AddIdeaConfiguration(Configuration.GetConnectionString("IdeaDatabase"), Configuration["Ideas:WordPressUrl"]);


            services.AddIdeaAuthSecurity(Configuration["Authorization:JwtSecretKey"],
                Configuration["Authorization:CoeAuthKey"], 
                Configuration["Authorization:CoeAuthIV"],
                Configuration["Ideas:WordPressUrl"]);

            services.AddMvc();

            services.AddAutoMapper();
        }


        protected void ConfigureCors(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(builder =>
            {
                (env.IsDevelopment() ? builder.AllowAnyOrigin() : builder.WithOrigins(Configuration["Ideas:WordPressUrl"]))
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

            ConfigureCors(app, env);

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
