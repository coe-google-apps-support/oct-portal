using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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

            Log.Information("Initiatives service started");

            return builder;
        }

        private static void ConfigureSerilog(IConfigurationRoot config)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
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
