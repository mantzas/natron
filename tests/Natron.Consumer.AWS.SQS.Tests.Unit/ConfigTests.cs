using FluentAssertions;

namespace Natron.Consumer.AWS.SQS.Tests.Unit;

public class ConfigTests
{
    [Fact]
    public void TestConfig()
    {
        const string url = "URL";
        Func<Batch, Task> fn = batch => Task.FromResult(0);

        var cfg = new Config(url, fn);
        cfg.QueueUrl.Should().Be(url);
        cfg.ProcessFunc.Should().Be(fn);
        cfg.VisibilityTimeout.Should().Be(10);
        cfg.WaitTimeSeconds.Should().Be(10);
        cfg.MaxNumberOfMessages.Should().Be(20);
        cfg.StatsInterval.Should().Be(10);
    }
}