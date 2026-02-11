namespace Natron.Http.Tests.Unit;

[Trait("Category", "Unit")]
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
#pragma warning disable CS0618
        var route = Route.TracedGet("TEMPLATE", _ => null!);
#pragma warning restore CS0618
        route.Verb.Should().Be("GET");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeTrue();
    }

    [Fact]
    public void Route_TracedPost()
    {
#pragma warning disable CS0618
        var route = Route.TracedPost("TEMPLATE", _ => null!);
#pragma warning restore CS0618
        route.Verb.Should().Be("POST");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeTrue();
    }

    [Fact]
    public void Route_TracedPut()
    {
#pragma warning disable CS0618
        var route = Route.TracedPut("TEMPLATE", _ => null!);
#pragma warning restore CS0618
        route.Verb.Should().Be("PUT");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeTrue();
    }

    [Fact]
    public void Route_TracedDelete()
    {
#pragma warning disable CS0618
        var route = Route.TracedDelete("TEMPLATE", _ => null!);
#pragma warning restore CS0618
        route.Verb.Should().Be("DELETE");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeTrue();
    }

    [Fact]
    public void Route_Get()
    {
        var route = Route.Get("TEMPLATE", _ => null!);
        route.Verb.Should().Be("GET");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeFalse();
    }

    [Fact]
    public void Route_Post()
    {
        var route = Route.Post("TEMPLATE", _ => null!);
        route.Verb.Should().Be("POST");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeFalse();
    }

    [Fact]
    public void Route_Put()
    {
        var route = Route.Put("TEMPLATE", _ => null!);
        route.Verb.Should().Be("PUT");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeFalse();
    }

    [Fact]
    public void Route_Delete()
    {
        var route = Route.Delete("TEMPLATE", _ => null!);
        route.Verb.Should().Be("DELETE");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeFalse();
    }

    [Fact]
    public void Route_Patch()
    {
        var route = Route.Patch("TEMPLATE", _ => null!);
        route.Verb.Should().Be("PATCH");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeFalse();
    }

    [Fact]
    public void Route_Head()
    {
        var route = Route.Head("TEMPLATE", _ => null!);
        route.Verb.Should().Be("HEAD");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeFalse();
    }

    [Fact]
    public void Route_Options()
    {
        var route = Route.Options("TEMPLATE", _ => null!);
        route.Verb.Should().Be("OPTIONS");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeFalse();
    }

    [Fact]
    public void Route_InstrumentedGet()
    {
        var route = Route.InstrumentedGet("TEMPLATE", _ => null!);
        route.Verb.Should().Be("GET");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeTrue();
    }

    [Fact]
    public void Route_InstrumentedPost()
    {
        var route = Route.InstrumentedPost("TEMPLATE", _ => null!);
        route.Verb.Should().Be("POST");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeTrue();
    }

    [Fact]
    public void Route_InstrumentedPut()
    {
        var route = Route.InstrumentedPut("TEMPLATE", _ => null!);
        route.Verb.Should().Be("PUT");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeTrue();
    }

    [Fact]
    public void Route_InstrumentedDelete()
    {
        var route = Route.InstrumentedDelete("TEMPLATE", _ => null!);
        route.Verb.Should().Be("DELETE");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeTrue();
    }

    [Fact]
    public void Route_InstrumentedPatch()
    {
        var route = Route.InstrumentedPatch("TEMPLATE", _ => null!);
        route.Verb.Should().Be("PATCH");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeTrue();
    }

    [Fact]
    public void Route_InstrumentedHead()
    {
        var route = Route.InstrumentedHead("TEMPLATE", _ => null!);
        route.Verb.Should().Be("HEAD");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeTrue();
    }

    [Fact]
    public void Route_InstrumentedOptions()
    {
        var route = Route.InstrumentedOptions("TEMPLATE", _ => null!);
        route.Verb.Should().Be("OPTIONS");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeTrue();
    }

    [Fact]
    public void Route_TracedPatch()
    {
#pragma warning disable CS0618
        var route = Route.TracedPatch("TEMPLATE", _ => null!);
#pragma warning restore CS0618
        route.Verb.Should().Be("PATCH");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeTrue();
    }

    [Fact]
    public void Route_TracedHead()
    {
#pragma warning disable CS0618
        var route = Route.TracedHead("TEMPLATE", _ => null!);
#pragma warning restore CS0618
        route.Verb.Should().Be("HEAD");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeTrue();
    }

    [Fact]
    public void Route_TracedOptions()
    {
#pragma warning disable CS0618
        var route = Route.TracedOptions("TEMPLATE", _ => null!);
#pragma warning restore CS0618
        route.Verb.Should().Be("OPTIONS");
        route.Template.Should().Be("TEMPLATE");
        route.Handler.Should().NotBeNull();
        route.Trace.Should().BeTrue();
    }
}