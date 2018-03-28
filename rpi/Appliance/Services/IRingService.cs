using System.Threading;
using System.Threading.Tasks;
using Appliance.Models.Ring;

namespace Appliance.Services
{
    public interface IRingService
    {
        Task<ActiveDing[]> PollActiveDings(CancellationToken cancellationToken);
    }
}