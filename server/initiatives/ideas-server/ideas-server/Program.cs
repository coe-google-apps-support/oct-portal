using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoE.Ideas.Core;
using CoE.Ideas.Core.Data;
using CoE.Ideas.Shared.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CoE.Ideas.Server
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
                        services.InitializeInitiativeDatabase();
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred while seeding the Initiatives database.");
                    }

                    try
                    {
                        services.InitializePermissionsDatabase(
                            (permissionName: Permissions.EditStatusDescription.ToString(), roleName: "Octava Business Analyst"));
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred while seeding the Permissions database.");
                    }
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

            //var serilogConfig = config.GetSection("Serilog");
            //var writeToConfig = serilogConfig?.GetSection("WriteTo");


            //if (writeToConfig != null)
            //{
            //    var writeToSinks = writeToConfig.GetChildren();
            //    if (writeToSinks != null)
            //    {
            //        foreach (var sink in writeToSinks)
            //        {
            //            var sinkName = sink.GetValue<string>("Name");
            //            var args = sink.GetSection("Args");

            //            // currently only AzureTableStorage is support for basic logging
            //            if ("AzureTableStorageWithProperties".Equals(sinkName, StringComparison.OrdinalIgnoreCase))
            //            {
            //                // override AzureTableStorageWithProperties to be without properties so we don't get all the noise the framework gives
            //                loggerConfig.WriteTo.AzureTableStorage(
            //                    args.GetValue<string>("connectionString"),
            //                    restrictedToMinimumLevel: args.GetValue("restrictedToMinimumLevel", Serilog.Events.LogEventLevel.Verbose),
            //                    storageTableName: args.GetValue<string>("storageTableName"),
            //                    writeInBatches: args.GetValue("writeInBatches", false),
            //                    batchPostingLimit: args.GetValue<int?>("batchPostingLimit", null));
            //            }
            //            else
            //            {
            //                // TODO: add regular providers...
            //            }
            //        }
            //    }
            //}



        }
    }
}
