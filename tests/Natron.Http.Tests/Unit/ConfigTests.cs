using Natron.Http;
using Natron.Http.Health;

namespace Natron.Http.Tests.Unit;

[Trait("Category", "Unit")]
public class ConfigTests
{
    [Fact]
    public void Constructor_Defaults()
    {
        var config = new Config();
        
        config.Urls.Should().Equal("http://0.0.0.0:50000", "https://0.0.0.0:50001");
        config.ShutdownTimeout.Should().Be(TimeSpan.FromSeconds(10));
        config.Routes.Should().BeEmpty();
        config.HealthChecks.Should().HaveCount(1);
    }

    [Fact]
    public void Constructor_CustomUrls()
    {
        var customUrls = new[] { "http://localhost:8080", "https://localhost:8443" };
        var config = new Config(urls: customUrls);
        
        config.Urls.Should().Equal(customUrls);
    }

    [Fact]
    public void Constructor_Throws_OnZeroShutdownTimeout()
    {
        var fun = () => new Config(shutdownTimeout: TimeSpan.Zero);
        fun.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_Throws_OnNegativeShutdownTimeout()
    {
        var fun = () => new Config(shutdownTimeout: TimeSpan.FromSeconds(-1));
        fun.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_CustomShutdownTimeout()
    {
        var customTimeout = TimeSpan.FromSeconds(30);
        var config = new Config(shutdownTimeout: customTimeout);
        
        config.ShutdownTimeout.Should().Be(customTimeout);
    }

    [Fact]
    public void UseRoutes_Succeeds()
    {
        var config = new Config();
        var route1 = Route.Get("route1", _ => null!);
        var route2 = Route.Post("route2", _ => null!);
        
        config.UseRoutes(route1, route2);
        
        config.Routes.Should().HaveCount(2);
        config.Routes.Should().Contain(route1);
        config.Routes.Should().Contain(route2);
    }

    [Fact]
    public void UseRoutes_Throws_OnNull()
    {
        var config = new Config();
        var fun = () => config.UseRoutes(null!);
        fun.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UseRoutes_ReplacesExistingRoutes()
    {
        var config = new Config();
        var route1 = Route.Get("route1", _ => null!);
        var route2 = Route.Post("route2", _ => null!);
        var route3 = Route.Put("route3", _ => null!);
        
        config.UseRoutes(route1, route2);
        config.Routes.Should().HaveCount(2);
        
        config.UseRoutes(route3);
        config.Routes.Should().HaveCount(1);
        config.Routes.Should().Contain(route3);
        config.Routes.Should().NotContain(route1);
        config.Routes.Should().NotContain(route2);
    }

    [Fact]
    public void UseHealthChecks_Succeeds()
    {
        var config = new Config();
        var healthCheck = HealthCheck.Default();
        
        config.UseHealthChecks(healthCheck);
        
        config.HealthChecks.Should().HaveCount(1);
        config.HealthChecks.Should().Contain(healthCheck);
    }

    [Fact]
    public void UseHealthChecks_Throws_OnNull()
    {
        var config = new Config();
        var fun = () => config.UseHealthChecks(null!);
        fun.Should().Throw<ArgumentException>();
    }
}
