using System;
using System.Globalization;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

namespace Natron.Config
{
    public sealed class TracingConfig
    {
        private const string DefaultAgentHost = "localhost";
        private const int DefaultAgentPort = 6831;
        private const string DefaultSamplerType = "probabilistic";
        private const double DefaultSamplerParam = 0.0;
        private const string EnvVarPrefix = "NATRON_TRACING";
        private const string AgentHostEnvVar = "NATRON_TRACING_AGENT_HOST";
        private const string AgentPortEnvVar = "NATRON_TRACING_AGENT_PORT";
        private const string SamplerTypeEnvVar = "NATRON_TRACING_SAMPLER_TYPE";
        private const string SamplerParaEnvVar = "NATRON_TRACING_SAMPLER_PARAM";

        public string AgentHost { get; } = DefaultAgentHost;
        public int AgentPort { get; } = DefaultAgentPort;
        public string SamplerType { get; } = DefaultSamplerType;
        public double SamplerParam { get; } = DefaultSamplerParam;

        public TracingConfig()
        {
        }

        private TracingConfig(string agentHost, int agentPort, string samplerType, double samplerParam)
        {
            AgentHost = agentHost;
            AgentPort = agentPort;
            SamplerType = samplerType;
            SamplerParam = samplerParam;
        }

        public static TracingConfig FromEnv()
        {
            var provider = new EnvironmentVariablesConfigurationProvider(EnvVarPrefix);
            provider.Load();
            if (!provider.TryGet(AgentHostEnvVar, out var agentHost))
            {
                agentHost = DefaultAgentHost;
            }

            var agentPort = DefaultAgentPort;

            if (provider.TryGet(AgentPortEnvVar, out var agentPortText))
            {
                if (!int.TryParse(agentPortText, NumberStyles.Integer, CultureInfo.InvariantCulture, out agentPort))
                {
                    throw new Exception($"Failed to parse {agentPortText} as int");
                }
            }

            if (!provider.TryGet(SamplerTypeEnvVar, out var samplerType))
            {
                samplerType = DefaultSamplerType;
            }

            var samplerParam = DefaultSamplerParam;

            if (!provider.TryGet(SamplerParaEnvVar, out var samplerParamText))
            {
                return new TracingConfig(agentHost, agentPort, samplerType, samplerParam);
            }

            if (!double.TryParse(samplerParamText, NumberStyles.Float, CultureInfo.InvariantCulture,
                out samplerParam))
            {
                throw new Exception($"Failed to parse {samplerParamText} as float");
            }

            return new TracingConfig(agentHost, agentPort, samplerType, samplerParam);
        }
    }
}