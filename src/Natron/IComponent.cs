using System.Threading;
using System.Threading.Tasks;

namespace Natron
{
    public interface IComponent
    {
        Task Run(CancellationToken cancelToken);
    }
}