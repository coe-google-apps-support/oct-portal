using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoE.Ideas.Shared.Extensions;
using CoE.Issues.Core;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CoE.Issues.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: true)
                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                .AddCommandLine(args)
                .Build();

            var appConfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                .AddCommandLine(args)
                .Build();

            ConfigureSerilog(appConfig);


            var builder = WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://0.0.0.0:5000")
                .UseConfiguration(config)
                .UseSerilog()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

#if DEBUG
            InitializeDatabase(builder);
#endif

            Log.Information("Initiatives service started");

            return builder;

        }

#if DEBUG
        private static void InitializeDatabase(IWebHost host)
        {
            // from https://stackoverflow.com/questions/39526595/entityframework-core-automatic-migrations
            // answer by Amac
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var environment = services.GetRequiredService<IHostingEnvironment>();

                if (environment.IsDevelopment())
                {

                    try
                    {
                        services.InitiativeServiceBusEmlatorDatabase();
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred while seeding the ServiceBusEmulator database.");
                    }

                    try
                    {
                        services.InitializeIssueDatabase();
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred while seeding the Initiatives database.");
                    }

                    //try
                    //{
                    //    services.InitializePermissionsDatabase(
                    //        (permissionName: Permissions.EditStatusDescription.ToString(), roleName: "Octava Business Analyst"));
                    //}
                    //catch (Exception ex)
                    //{
                    //    var logger = services.GetRequiredService<ILogger<Program>>();
                    //    logger.LogError(ex, "An error occurred while seeding the Permissions database.");
                    //}
                }
            }

            host.Run();
        }
#endif

        private static void ConfigureSerilog(IConfigurationRoot config)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                // .Net Core is too noisy! Just should our own logging!
                .Filter.ByIncludingOnly(Serilog.Filters.Matching.WithProperty("Application"))
                .CreateLogger();

        }


    }
}
