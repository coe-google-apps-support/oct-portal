using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoE.Ideas.Shared.Extensions;
using CoE.Issues.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CoE.Issues.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }
        private IHostingEnvironment HostingEnvironment { get; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // configure application specific logging
            services.ConfigureLogging(Configuration, "Issues-Server", useSqlServer: HostingEnvironment.IsDevelopment());


            services.AddLocalIssueConfiguration(
                Configuration.GetConnectionString("IssueDatabase"),
                Configuration["WordPress:Url"]);

            services.AddPermissionSecurity(Configuration.GetConnectionString("WordPressDatabase"));
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


            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
