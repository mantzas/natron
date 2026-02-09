namespace Natron;

public interface IComponent
{
    Task RunAsync(CancellationToken cancellationToken);
}