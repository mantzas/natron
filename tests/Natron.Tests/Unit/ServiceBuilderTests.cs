using System;
using System.Threading;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Natron.Tests.Unit;

[Trait("Category", "Unit")]
public class ServiceBuilderTests
{
    [Fact]
    public void Create_Throws_OnNullName()
    {
        var fun = () => ServiceBuilder.Create(null!, Substitute.For<ILoggerFactory>(), new CancellationTokenSource());
        fun.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Throws_OnEmptyName()
    {
        var fun = () => ServiceBuilder.Create("", Substitute.For<ILoggerFactory>(), new CancellationTokenSource());
        fun.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Throws_OnWhitespaceName()
    {
        var fun = () => ServiceBuilder.Create("  ", Substitute.For<ILoggerFactory>(), new CancellationTokenSource());
        fun.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Throws_OnNullLoggerFactory()
    {
        var fun = () => ServiceBuilder.Create("test", null!, new CancellationTokenSource());
        fun.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Throws_OnNullCts()
    {
        var fun = () => ServiceBuilder.Create("test", Substitute.For<ILoggerFactory>(), null!);
        fun.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Succeeds()
    {
        var builder = ServiceBuilder.Create("test", Substitute.For<ILoggerFactory>(), new CancellationTokenSource());
        builder.Should().NotBeNull();
    }

    [Fact]
    public void ConfigureComponents_CanBeCalledMultipleTimes()
    {
        var lf = Substitute.For<ILoggerFactory>();
        var cts = new CancellationTokenSource();
        var cmp1 = Substitute.For<IComponent>();
        var cmp2 = Substitute.For<IComponent>();
        
        var service = ServiceBuilder.Create("test", lf, cts)
            .ConfigureComponents(cmp1)
            .ConfigureComponents(cmp2)
            .Build();
        
        service.Should().NotBeNull();
    }

    [Fact]
    public void Build_ReturnsService()
    {
        var lf = Substitute.For<ILoggerFactory>();
        var cts = new CancellationTokenSource();
        var cmp = Substitute.For<IComponent>();
        
        var service = ServiceBuilder.Create("test", lf, cts)
            .ConfigureComponents(cmp)
            .Build();
        
        service.Should().NotBeNull();
    }
}
