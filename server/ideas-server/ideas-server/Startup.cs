using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Shared.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
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

                //TODO: Load integration projects to simulate receiving service bus events?
                var svcReceiver = app.ApplicationServices.GetService<CoE.Ideas.Core.ServiceBus.SynchronousInitiativeMessageReceiver>();
                if (svcReceiver != null)
                {
                    //This doesn't work because the integration projects have nuget dependencies that are
                    //not referenced by the ideas-server project (throws error...)
                    //LoadIntegrationComponents(app, svcReceiver);
                    // svcReceiver.CreatedHandlers.Add(/* RemedyListener */)
                    // etc.
                    // but we have to load the assemblies dynamically with Reflection because
                    // I don't want to reference them directly in this project.
                }
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


            ////var rewriteOptions = new RewriteOptions();
            ////rewriteOptions.AddRewrite(@".*", "/index.html", false);
            ////app.UseRewriter(rewriteOptions);

            //app.Use(async (context, next) =>
            //{
            //    var m = app.ApplicationServices.GetService<Microsoft.AspNetCore.StaticFiles.StaticFileMiddleware>();
            //    //app.ApplicationServices.GetService<StaticFileContext>();
            //   // await m.Invoke(context);

            //    await next.Invoke();
            //});


        }

        //private void LoadIntegrationComponents(IApplicationBuilder app, SynchronousInitiativeMessageReceiver svcReceiver)
        //{
        //    // This gets the "server" root directory of the github source
        //    var dir = new System.IO.DirectoryInfo(System.IO.Directory.GetCurrentDirectory()).Parent.Parent;
        //    var allDlls = dir.EnumerateFiles("*.dll", System.IO.SearchOption.AllDirectories)
        //        .Where(f => f.Directory.Name == "netcoreapp2.0")
        //        .GroupBy(x => x.Name)
        //        .Select(x => x.OrderByDescending(y => y.LastWriteTimeUtc).First())
        //        .ToList();

        //    // "ideas-integration-remedy"
        //    var ideasIntegrationRemedy = allDlls.FirstOrDefault(x => x.Name == "ideas-integration-remedy.dll");
        //    if (ideasIntegrationRemedy != null)
        //    {
        //        LoadIntegrationAssembly(ideasIntegrationRemedy.FullName, "CoE.Ideas.Remedy.Program", svcReceiver);
        //    }
        //}

        //private void LoadIntegrationAssembly(string assemblyFullPath, string entryType, SynchronousInitiativeMessageReceiver svcReceiver)
        //{
        //    try
        //    {
        //        // from https://github.com/dotnet/corefx/issues/11639
        //        var fileNameWithOutExtension = System.IO.Path.GetFileNameWithoutExtension(assemblyFullPath);
        //        var fileName = System.IO.Path.GetFileName(assemblyFullPath);
        //        var directory = System.IO.Path.GetDirectoryName(assemblyFullPath);

        //        var inCompileLibraries = Microsoft.Extensions.DependencyModel.DependencyContext.Default.CompileLibraries.Any(l => l.Name.Equals(fileNameWithOutExtension, StringComparison.OrdinalIgnoreCase));
        //        var inRuntimeLibraries = Microsoft.Extensions.DependencyModel.DependencyContext.Default.RuntimeLibraries.Any(l => l.Name.Equals(fileNameWithOutExtension, StringComparison.OrdinalIgnoreCase));

        //        var assembly = (inCompileLibraries || inRuntimeLibraries)
        //            ? System.Reflection.Assembly.Load(new System.Reflection.AssemblyName(fileNameWithOutExtension))
        //            : System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyFullPath);

        //        if (assembly != null)
        //        {
        //            var entry = assembly.GetType(entryType, throwOnError: false, ignoreCase: false);

        //            if (entry != null)
        //            {
        //                var entryMethod = entry.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
        //                    .Select(x => new { Method = x, Parameters = x.GetParameters() })
        //                    .SingleOrDefault(x => x.Parameters.Length == 3
        //                             && x.Parameters[0].ParameterType == typeof(string[])
        //                             && x.Parameters[1].ParameterType == typeof(string)
        //                             && x.Parameters[2].ParameterType == typeof(SynchronousInitiativeMessageReceiver));
        //                if (entryMethod != null)
        //                {
        //                    //LoadReferencedAssemblies(assembly, fileName, directory);

        //                    // invoke the method on a new thread
        //                    System.Threading.ThreadPool.QueueUserWorkItem(x =>
        //                    {
        //                        entryMethod.Method.Invoke(null, new object[] { new string[] { }, new System.IO.FileInfo(assemblyPath).DirectoryName, svcReceiver });
        //                    });
        //                }
        //            }

        //        }
        //    }
        //    catch (Exception err)
        //    {
        //        throw;
        //    }

        //}

        ////private static void LoadReferencedAssemblies(System.Reflection.Assembly assembly, string fileName, string directory)
        ////{
        ////    var filesInDirectory = System.IO.Directory.GetFiles(directory).Where(x => x != fileName).Select(x => System.IO.Path.GetFileNameWithoutExtension(x)).ToList();
        ////    var references = assembly.GetReferencedAssemblies();

        ////    foreach (var reference in references)
        ////    {
        ////        if (filesInDirectory.Contains(reference.Name))
        ////        {
        ////            var loadFileName = reference.Name + ".dll";
        ////            var path = System.IO.Path.Combine(directory, loadFileName);
        ////            var loadedAssembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
        ////            if (loadedAssembly != null)
        ////                LoadReferencedAssemblies(loadedAssembly, loadFileName, directory);
        ////        }
        ////    }

        ////}
    }
}
