using Natron.AWS.Consumer;

namespace Natron.AWS.Tests.Unit.SQS.Consumer;

[Trait("Category", "Unit")]
public class ConfigTests
{
    [Fact]
    public void TestConfig()
    {
        const string url = "URL";

        var cfg = new Config(url, Fn);
        cfg.QueueUrl.Should().Be(url);
        cfg.ProcessFunc.Should().Be((Func<Batch, Task>)Fn);
        cfg.VisibilityTimeout.Should().Be(10);
        cfg.WaitTimeSeconds.Should().Be(10);
        cfg.MaxNumberOfMessages.Should().Be(10);
        cfg.ProcessingStrategy.Should().Be(ProcessingStrategy.Crash);
        return;

        Task Fn(Batch _)
        {
            return Task.FromResult(0);
        }
    }

    [Fact]
    public void Config_Throws_OnNullQueueUrl()
    {
        var fun = () => new Config(null!, Fn);
        fun.Should().Throw<ArgumentException>();
        return;

        Task Fn(Batch _)
        {
            return Task.FromResult(0);
        }
    }

    [Fact]
    public void Config_Throws_OnEmptyQueueUrl()
    {
        var fun = () => new Config("", Fn);
        fun.Should().Throw<ArgumentException>();
        return;

        Task Fn(Batch _)
        {
            return Task.FromResult(0);
        }
    }

    [Fact]
    public void Config_Throws_OnNullProcessFunc()
    {
        var fun = () => new Config("URL", null!);
        fun.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Config_Throws_OnZeroVisibilityTimeout()
    {
        var fun = () => new Config("URL", Fn, visibilityTimeout: 0);
        fun.Should().Throw<ArgumentException>();
        return;

        Task Fn(Batch _)
        {
            return Task.FromResult(0);
        }
    }

    [Fact]
    public void Config_Throws_OnNegativeVisibilityTimeout()
    {
        var fun = () => new Config("URL", Fn, visibilityTimeout: -1);
        fun.Should().Throw<ArgumentException>();
        return;

        Task Fn(Batch _)
        {
            return Task.FromResult(0);
        }
    }

    [Fact]
    public void Config_Throws_OnZeroWaitTimeSeconds()
    {
        var fun = () => new Config("URL", Fn, waitTimeSeconds: 0);
        fun.Should().Throw<ArgumentException>();
        return;

        Task Fn(Batch _)
        {
            return Task.FromResult(0);
        }
    }

    [Fact]
    public void Config_Throws_OnZeroMaxNumberOfMessages()
    {
        var fun = () => new Config("URL", Fn, maxNumberOfMessages: 0);
        fun.Should().Throw<ArgumentException>();
        return;

        Task Fn(Batch _)
        {
            return Task.FromResult(0);
        }
    }

    [Fact]
    public void Config_Throws_OnInvalidProcessingStrategy()
    {
        var fun = () => new Config("URL", Fn, processingStrategy: (ProcessingStrategy)99);
        fun.Should().Throw<ArgumentException>();
        return;

        Task Fn(Batch _)
        {
            return Task.FromResult(0);
        }
    }

    [Fact]
    public void Config_LogAndContinue()
    {
        var cfg = new Config("URL", Fn, processingStrategy: ProcessingStrategy.LogAndContinue);
        cfg.ProcessingStrategy.Should().Be(ProcessingStrategy.LogAndContinue);
        return;

        Task Fn(Batch _)
        {
            return Task.FromResult(0);
        }
    }
}