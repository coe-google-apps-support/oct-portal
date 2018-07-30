using System;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace CoE.Issues.Remedy.WorkOrder.Watcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var startup = new Startup(config);

            TimeSpan pollInterval = TimeSpan.Parse(config["Remedy:PollInterval"]);
            while (true)
            {
                try
                {
                    startup.Start().GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    // gobble exceptions
                    Trace.TraceError($"Polling error: { e }");
                }
                finally
                {
                    Thread.Sleep(pollInterval);
                }
            }
        }
    }
}
