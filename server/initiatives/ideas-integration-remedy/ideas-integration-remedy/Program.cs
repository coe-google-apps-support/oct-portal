using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading;
using System.ServiceModel;
using Serilog;

namespace CoE.Ideas.Remedy
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            //Log.Logger = new LoggerConfiguration()
            //    .ReadFrom.Configuration(config)
            //    // .Net Core is too noisy! Just should our own logging!
            //    //.Filter.ByIncludingOnly(Serilog.Filters.Matching.WithProperty("Application"))
            //    .CreateLogger();

            //Log.Information("Starting Remedy integration...");

            new Startup(config);

            // now block forever
            new ManualResetEvent(false).WaitOne();
        }
    }
}
