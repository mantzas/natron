using Microsoft.AspNetCore.Http;
using ValidDotNet;

namespace Natron.Http
{
    public sealed class Route
    {
        public string Verb { get; }
        public string Template { get; }
        public RequestDelegate Handler { get; }
        public bool Trace { get; }

        public Route(string verb, string template, RequestDelegate handler, bool trace)
        {
            Verb = verb.ThrowIfNullOrWhitespace(nameof(verb));
            Template = template.ThrowIfNullOrWhitespace(nameof(template));
            Handler = handler.ThrowIfNull(nameof(handler));
            Trace = trace;
        }
    }
}