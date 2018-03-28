using System.Threading;
using System.Threading.Tasks;

namespace Appliance.Controllers
{
    public interface IGarageController
    {
        Task PressGarageDoorRemoteButton(CancellationToken cancellationToken);
    }
}