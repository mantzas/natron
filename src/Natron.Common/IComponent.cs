namespace Natron.Common;

public interface IComponent
{
    Task RunAsync(CancellationToken cancelToken);
}