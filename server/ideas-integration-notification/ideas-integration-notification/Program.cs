using CoE.Ideas.Core;
using CoE.Ideas.Core.ServiceBus;
using CoE.Ideas.Core.WordPress;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Threading;

namespace CoE.Ideas.Integration.Notification
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

            // Configure Serilog here to ensure we capture startup errors
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            new Startup(config);

            // now block forever
            // but I don't think the code will ever get here anyway...
            new ManualResetEvent(false).WaitOne();
        }



    }
}
