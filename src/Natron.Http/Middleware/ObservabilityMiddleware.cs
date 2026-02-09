using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Prometheus;

namespace Natron.Http.Middleware;

public sealed class ObservabilityMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly Histogram HttpRequestDurationHistogram = CreateHttpRequestHistogram();

    public ObservabilityMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var watch = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            watch.Stop();
            
            var routeTemplate = context.GetEndpoint()?.Metadata.GetMetadata<RouteEndpoint>()?.RoutePattern.RawText ?? "unknown";
            
            HttpRequestDurationHistogram
                .WithLabels(context.Response.StatusCode.ToString(CultureInfo.InvariantCulture),
                    context.Request.Method, routeTemplate)
                .Observe(watch.Elapsed.TotalSeconds);
        }
    }

    private static Histogram CreateHttpRequestHistogram()
    {
        return Metrics.CreateHistogram(
            "http_request_duration_seconds",
            "The duration of HTTP requests processed by an ASP.NET Core application.",
            new HistogramConfiguration
            {
                LabelNames = ["code", "method", "path"],
                Buckets = [0.001, 0.005, 0.010, 0.050, 0.1, 0.2, 1.0, 2.0, 5.0, 10.0]
            });
    }
}
