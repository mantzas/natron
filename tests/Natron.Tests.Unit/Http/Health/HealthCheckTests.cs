using System;
using FluentAssertions;
using Natron.Http.Health;
using Xunit;

namespace Natron.Tests.Unit.Http.Health;

public class HealthCheckTests
{
    [Fact]
    public void HealthCheck_Constructor_Throws_OnNullName()
    {
        var fun = () => new HealthCheck(null!, null!);
        fun.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void HealthCheck_Constructor_Throws_OnNullInstance()
    {
        var fun = () => new HealthCheck("NAME", null!);
        fun.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void HealthCheck_Constructor_Succeeds()
    {
        var route = new HealthCheck("NAME", new DefaultHealthCheck());
        route.Should().NotBeNull();
        route.Name.Should().Be("NAME");
        route.Instance.Should().NotBeNull();
    }
}