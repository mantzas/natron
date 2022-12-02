using Microsoft.Extensions.Logging;
using Natron;
using Natron.Example;
using Natron.Http;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

var loggerFactory = new LoggerFactory();
loggerFactory.AddSerilog();
loggerFactory.CreateLogger<Program>().LogInformation("creating service");

var cts = new CancellationTokenSource();

var config = new HttpConfig();
config.UseHealthChecks();

await ServiceBuilder.Create("example", loggerFactory, cts)
    .ConfigureComponents(new Component(loggerFactory, config), new TestComponent())
    .Build().RunAsync();