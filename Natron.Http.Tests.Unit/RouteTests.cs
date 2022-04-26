using System;
using FluentAssertions;
using Xunit;

namespace Natron.Http.Tests.Unit;

public class RouteTests
{
    [Fact]
    public void Route_Constructor_Throws_OnNullVerb()
    {
        var fun = () => new Route(null!, null!, null!, false);
        fun.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Route_Constructor_Throws_OnNullTemplate()
    {
        var fun = () => new Route("GET", null!, null!, false);
        fun.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Route_Constructor_Throws_OnNullHandler()
    {
        var fun = () => new Route("GET", "TEMPLATE", null!, false);
        fun.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Route_Constructor_Succeeds()
    {
        var route = new Route("GET", "TEMPLATE", _ => null!, true);
        route.Should().NotBeNull();
        route.Verb.Should().Be("GET");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeTrue();
    }

    [Fact]
    public void Route_TracedGet()
    {
        var route = Route.TracedGet("TEMPLATE", _ => null!);
        route.Verb.Should().Be("GET");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeTrue();
    }

    [Fact]
    public void Route_TracedPost()
    {
        var route = Route.TracedPost("TEMPLATE", _ => null!);
        route.Verb.Should().Be("POST");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeTrue();
    }

    [Fact]
    public void Route_TracedPut()
    {
        var route = Route.TracedPut("TEMPLATE", _ => null!);
        route.Verb.Should().Be("PUT");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeTrue();
    }

    [Fact]
    public void Route_TracedDelete()
    {
        var route = Route.TracedDelete("TEMPLATE", _ => null!);
        route.Verb.Should().Be("DELETE");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeTrue();
    }
}