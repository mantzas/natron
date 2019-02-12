using System;
using FluentAssertions;
using Natron.Http;
using Natron.Http.Health;
using Xunit;

namespace Natron.Tests.Unit.Http
{
    public class HealthCheckTests
    {
        [Fact]
        public void HealthCheck_Constructor_Throws_OnNullName()
        {
            Func<HealthCheck> fun = () => new HealthCheck(null, null);
            fun.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void HealthCheck_Constructor_Throws_OnNullInstance()
        {
            Func<HealthCheck> fun = () => new HealthCheck("NAME", null);
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
}