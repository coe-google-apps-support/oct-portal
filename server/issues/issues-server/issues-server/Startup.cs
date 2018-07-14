using CoE.Ideas.Shared.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Newtonsoft.Json;
using CoE.Issues.Core;

namespace CoE.Issues.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        private IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // configure application specific logging
            services.ConfigureLogging(Configuration, "Server", useSqlServer: HostingEnvironment.IsDevelopment());

            services.AddLocalIssueConfiguration(
                Configuration.GetConnectionString("IssueDatabase"),
                Configuration["WordPress:Url"]);

            services.AddPermissionSecurity(Configuration.GetConnectionString("WordPressDatabase"));

            System.Diagnostics.Trace.WriteLine("Does this get hit?");

            services.AddWordPressServices(Configuration.GetConnectionString("WordPressDatabase"));



            if (HostingEnvironment.IsDevelopment())
            {
                services.AddWordPressSecurity(Configuration.GetSection("WordPress")
#if DEBUG
                    , staticDevUserName: "Snow White"
                    , staticDevEmail: "snow.white@edmonton.ca"
#endif
                    );
            }
            else
            {
                services.AddWordPressSecurity(Configuration.GetSection("WordPress"));
            }

            services.AddIssueMessaging(Configuration.GetConnectionString("IdeaServiceBus"),
                Configuration["Ideas:ServiceBusTopic"]);

            services.AddMvc().AddJsonOptions(options => {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

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
                    .WithExposedHeaders("X-Is-Last-Page", "X-Total-Count", "Can-Edit")
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

            app.UseStaticFiles();
            var d = new DefaultFilesOptions();
            d.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(d);

            app.UseMvc(routes =>
            {
                routes.MapRoute("api", "api/{action}", defaults: new { controller = "Ideas" });
                routes.MapRoute("Spa", "{*url}", defaults: new { controller = "Home", action = "Spa" });
            });
        }
    }
}
