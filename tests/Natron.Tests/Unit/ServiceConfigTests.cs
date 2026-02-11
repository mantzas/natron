using System;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Natron.Tests.Unit;

[Trait("Category", "Unit")]
public class ServiceConfigTests
{
    [Fact]
    public void Constructor_Throws_OnNullName()
    {
        var fun = () => new ServiceConfig(null!, Substitute.For<ILoggerFactory>(), new CancellationTokenSource());
        fun.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_Throws_OnNullLoggerFactory()
    {
        var fun = () => new ServiceConfig("test", null!, new CancellationTokenSource());
        fun.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_Throws_OnNullCts()
    {
        var fun = () => new ServiceConfig("test", Substitute.For<ILoggerFactory>(), null!);
        fun.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_Succeeds()
    {
        var lf = Substitute.For<ILoggerFactory>();
        var cts = new CancellationTokenSource();
        
        var config = new ServiceConfig("test", lf, cts);
        
        config.Should().NotBeNull();
        config.Name.Should().Be("test");
        config.LoggerFactory.Should().Be(lf);
        config.CancellationTokenSource.Should().Be(cts);
        config.Components.Should().BeEmpty();
    }

    [Fact]
    public void UseComponents_Throws_OnNull()
    {
        var config = new ServiceConfig("test", Substitute.For<ILoggerFactory>(), new CancellationTokenSource());
        var fun = () => config.UseComponents(null!);
        fun.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UseComponents_Throws_OnEmpty()
    {
        var config = new ServiceConfig("test", Substitute.For<ILoggerFactory>(), new CancellationTokenSource());
        var fun = () => config.UseComponents();
        fun.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UseComponents_Succeeds()
    {
        var config = new ServiceConfig("test", Substitute.For<ILoggerFactory>(), new CancellationTokenSource());
        var cmp = Substitute.For<IComponent>();
        
        config.UseComponents(cmp);
        
        config.Components.Should().NotBeEmpty();
        config.Components.Should().HaveCount(1);
        config.Components.First().Should().Be(cmp);
    }

    [Fact]
    public void UseComponents_ReplacesExistingComponents()
    {
        var config = new ServiceConfig("test", Substitute.For<ILoggerFactory>(), new CancellationTokenSource());
        var cmp1 = Substitute.For<IComponent>();
        var cmp2 = Substitute.For<IComponent>();
        
        config.UseComponents(cmp1);
        config.Components.Should().HaveCount(1);
        config.Components.First().Should().Be(cmp1);
        
        config.UseComponents(cmp2);
        config.Components.Should().HaveCount(1);
        config.Components.First().Should().Be(cmp2);
    }
}
