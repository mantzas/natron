using FluentAssertions;

namespace Natron.Consumer.AWS.SQS.Tests.Unit;

[Trait("Category", "Unit")]
public class ConfigTests
{
    [Fact]
    public void TestConfig()
    {
        const string url = "URL";

        Task Fn(Batch _)
        {
            return Task.FromResult(0);
        }

        var cfg = new Config(url, Fn);
        cfg.QueueUrl.Should().Be(url);
        cfg.ProcessFunc.Should().Be((Func<Batch, Task>)Fn);
        cfg.VisibilityTimeout.Should().Be(10);
        cfg.WaitTimeSeconds.Should().Be(10);
        cfg.MaxNumberOfMessages.Should().Be(20);
        cfg.StatsInterval.Should().Be(10);
    }
}