using Microsoft.AspNetCore.Http;
using ValidDotNet;

namespace Natron.Http;

public record Route
{
    public Route(string verb, string template, RequestDelegate handler, bool trace)
    {
        Verb = verb.ThrowIfNullOrWhitespace();
        Template = template.ThrowIfNullOrWhitespace();
        Handler = handler.ThrowIfNull();
        Trace = trace;
    }

    public string Verb { get; }
    public string Template { get; }
    public RequestDelegate Handler { get; }
    public bool Trace { get; }

    public static Route TracedGet(string template, RequestDelegate handler)
    {
        return new Route(RouteMethod.GET, template, handler, true);
    }

    public static Route TracedPost(string template, RequestDelegate handler)
    {
        return new Route(RouteMethod.POST, template, handler, true);
    }

    public static Route TracedPut(string template, RequestDelegate handler)
    {
        return new Route(RouteMethod.PUT, template, handler, true);
    }

    public static Route TracedDelete(string template, RequestDelegate handler)
    {
        return new Route(RouteMethod.DELETE, template, handler, true);
    }
}