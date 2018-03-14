using Microsoft.Extensions.Configuration;
using Serilog;
using System.IO;
using System.Threading;

namespace CoE.Ideas.Integration.Logger
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
            new ManualResetEvent(false).WaitOne();
        }
    }
}
