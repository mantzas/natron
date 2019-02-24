using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Natron.Http;
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

            var config = new HttpConfig();
            config.Routes.Add(Route.TracedGet("/test", context => context.Response.WriteAsync("test")));

            await ServiceBuilder.Create(loggerFactory).ConfigureHttp(config).Build().RunAsync();
        }
    }
}