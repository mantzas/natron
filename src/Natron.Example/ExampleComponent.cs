namespace Natron.Example;

public class TestComponent : IComponent
{
    public async Task RunAsync(CancellationToken cancelToken)
    {
        while (!cancelToken.IsCancellationRequested) await Task.Delay(10000, cancelToken);
    }
}