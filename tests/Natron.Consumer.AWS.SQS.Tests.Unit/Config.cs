using Amazon.SQS.Model;
using FluentAssertions;

namespace Natron.Consumer.AWS.SQS.Tests.Unit;

public class UnitTest1
{
    [Fact]
    public void TestConfig()
    {
        const string url = "URL";
        Action<Message> fn = _ => { };

        var cfg = new Config(url, fn);
        cfg.QueueUrl.Should().Be(url);
        cfg.ProcessFunc.Should().Be(fn);
        cfg.VisibilityTimeout.Should().Be(10);
        cfg.WaitTimeSeconds.Should().Be(10);
        cfg.MaxNumberOfMessages.Should().Be(20);
    }
}