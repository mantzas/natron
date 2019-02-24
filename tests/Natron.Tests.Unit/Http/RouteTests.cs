using System;
using FluentAssertions;
using Natron.Http;
using Xunit;

namespace Natron.Tests.Unit.Http
{
    public class RouteTests
    {
        [Fact]
        public void Route_Constructor_Throws_OnNullVerb()
        {
            Func<Route> fun = () => new Route(null, null, null, false);
            fun.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Route_Constructor_Throws_OnNullTemplate()
        {
            Func<Route> fun = () => new Route("GET", null, null, false);
            fun.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Route_Constructor_Throws_OnNullHandler()
        {
            Func<Route> fun = () => new Route("GET", "TEMPLATE", null, false);
            fun.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Route_Constructor_Succeeds()
        {
            var route = new Route("GET", "TEMPLATE", context => null, true);
            route.Should().NotBeNull();
            route.Verb.Should().Be("GET");
            route.Template.Should().Be("TEMPLATE");
            route.Handler.Should().NotBeNull();
            route.Trace.Should().BeTrue();
        }

        [Fact]
        public void Route_TracedGet()
        {
            var route = Route.TracedGet("TEMPLATE", context => null);
            route.Verb.Should().Be("GET");
            route.Template.Should().Be("TEMPLATE");
            route.Handler.Should().NotBeNull();
            route.Trace.Should().BeTrue();
        }

        [Fact]
        public void Route_TracedPost()
        {
            var route = Route.TracedPost("TEMPLATE", context => null);
            route.Verb.Should().Be("POST");
            route.Template.Should().Be("TEMPLATE");
            route.Handler.Should().NotBeNull();
            route.Trace.Should().BeTrue();
        }

        [Fact]
        public void Route_TracedPut()
        {
            var route = Route.TracedPut("TEMPLATE", context => null);
            route.Verb.Should().Be("PUT");
            route.Template.Should().Be("TEMPLATE");
            route.Handler.Should().NotBeNull();
            route.Trace.Should().BeTrue();
        }

        [Fact]
        public void Route_TracedDelete()
        {
            var route = Route.TracedDelete("TEMPLATE", context => null);
            route.Verb.Should().Be("DELETE");
            route.Template.Should().Be("TEMPLATE");
            route.Handler.Should().NotBeNull();
            route.Trace.Should().BeTrue();
        }
    }
}