using Microsoft.AspNetCore.Http;
using ValidDotNet;

namespace Natron.Http;

public class Route
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

    // Untraced factory methods
    public static Route Get(string template, RequestDelegate handler)
    {
        return new Route(RouteMethod.Get, template, handler, false);
    }

    public static Route Post(string template, RequestDelegate handler)
    {
        return new Route(RouteMethod.Post, template, handler, false);
    }

    public static Route Put(string template, RequestDelegate handler)
    {
        return new Route(RouteMethod.Put, template, handler, false);
    }

    public static Route Delete(string template, RequestDelegate handler)
    {
        return new Route(RouteMethod.Delete, template, handler, false);
    }

    public static Route Patch(string template, RequestDelegate handler)
    {
        return new Route(RouteMethod.Patch, template, handler, false);
    }

    public static Route Head(string template, RequestDelegate handler)
    {
        return new Route(RouteMethod.Head, template, handler, false);
    }

    public static Route Options(string template, RequestDelegate handler)
    {
        return new Route(RouteMethod.Options, template, handler, false);
    }

    // Instrumented/traced factory methods (renamed from "Traced" to "Instrumented")
    public static Route InstrumentedGet(string template, RequestDelegate handler)
    {
        return new Route(RouteMethod.Get, template, handler, true);
    }

    public static Route InstrumentedPost(string template, RequestDelegate handler)
    {
        return new Route(RouteMethod.Post, template, handler, true);
    }

    public static Route InstrumentedPut(string template, RequestDelegate handler)
    {
        return new Route(RouteMethod.Put, template, handler, true);
    }

    public static Route InstrumentedDelete(string template, RequestDelegate handler)
    {
        return new Route(RouteMethod.Delete, template, handler, true);
    }

    public static Route InstrumentedPatch(string template, RequestDelegate handler)
    {
        return new Route(RouteMethod.Patch, template, handler, true);
    }

    public static Route InstrumentedHead(string template, RequestDelegate handler)
    {
        return new Route(RouteMethod.Head, template, handler, true);
    }

    public static Route InstrumentedOptions(string template, RequestDelegate handler)
    {
        return new Route(RouteMethod.Options, template, handler, true);
    }

    // Keep old "Traced" methods for backward compatibility (marked as Obsolete)
    [Obsolete("Use InstrumentedGet instead")]
    public static Route TracedGet(string template, RequestDelegate handler)
    {
        return InstrumentedGet(template, handler);
    }

    [Obsolete("Use InstrumentedPost instead")]
    public static Route TracedPost(string template, RequestDelegate handler)
    {
        return InstrumentedPost(template, handler);
    }

    [Obsolete("Use InstrumentedPut instead")]
    public static Route TracedPut(string template, RequestDelegate handler)
    {
        return InstrumentedPut(template, handler);
    }

    [Obsolete("Use InstrumentedDelete instead")]
    public static Route TracedDelete(string template, RequestDelegate handler)
    {
        return InstrumentedDelete(template, handler);
    }
}