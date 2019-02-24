using System.Collections.Generic;
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
            var routes = new List<Route>
            {
                new Route("GET","/test", context => context.Response.WriteAsync("test"),true),
            };
            await ServiceBuilder.Create(loggerFactory).ConfigureRoutes(routes).Build().RunAsync();
        }
    }
}