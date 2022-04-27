using Microsoft.Extensions.Logging;
using Natron;
using Natron.Example;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

var loggerFactory = new LoggerFactory();
loggerFactory.AddSerilog();
loggerFactory.CreateLogger<Program>().LogInformation("creating service");

var cts = new CancellationTokenSource();

await ServiceBuilder.Create("example", loggerFactory, cts).ConfigureComponents(new TestComponent()).Build().RunAsync();