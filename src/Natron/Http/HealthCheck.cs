using Microsoft.Extensions.Diagnostics.HealthChecks;
using ValidDotNet;

namespace Natron.Http
{
    public class HealthCheck
    {
        public string Name { get; }
        public IHealthCheck Instance { get; }

        public HealthCheck(string name, IHealthCheck instance)
        {
            Name = name.ThrowIfNullOrWhitespace(nameof(name));
            Instance = instance.ThrowIfNull(nameof(instance));
        }
    }
}