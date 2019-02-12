using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Natron.Example
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddSerilog();
            loggerFactory.CreateLogger<Program>().LogInformation("Creating service");
            await ServiceBuilder.Create(loggerFactory).Build().Run();
        }
    }
}