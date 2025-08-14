namespace Natron;

public interface IService : IDisposable
{
    Task RunAsync();
}
