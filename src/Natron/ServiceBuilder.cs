using System.Threading;
using Microsoft.Extensions.Logging;
using Natron.Config;
using Natron.Http;

namespace Natron
{
    public sealed class ServiceBuilder
    {
        private readonly ServiceConfig _config;

        private ServiceBuilder(string name, ILoggerFactory loggerFactory)
        {
            _config = new ServiceConfig(name, loggerFactory);
        }

        public static ServiceBuilder Create(string name, ILoggerFactory loggerFactory)
        {
            return new ServiceBuilder(name, loggerFactory);
        }

        public ServiceBuilder ConfigureCancellationTokenSource(CancellationTokenSource source)
        {
            _config.UseCancellationTokenSource(source);
            return this;
        }

        public ServiceBuilder ConfigureHttp(HttpConfig config)
        {
            _config.UseComponents(new Component(_config.LoggerFactory, config));
            return this;
        }

        public ServiceBuilder ConfigureTracingFromEnvVars()
        {
            _config.UseTracingConfig(TracingConfig.FromEnv());
            return this;
        }

        public ServiceBuilder ConfigureComponents(params IComponent[] components)
        {
            _config.UseComponents(components);
            return this;
        }

        public Service Build()
        {
            return new Service(_config);
        }
    }
}