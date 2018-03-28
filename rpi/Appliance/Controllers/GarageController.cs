using System.Threading;
using System.Threading.Tasks;

namespace Appliance.Controllers
{
    /// <summary>
    /// Control garage door remote interaction
    /// </summary>
    public class GarageController : IGarageController
    {
        private readonly IRelaysController _relaysController;

        public GarageController(IRelaysController relaysController)
        {
            _relaysController = relaysController;
        }

        public async Task PressGarageDoorRemoteButton(CancellationToken cancellationToken)
        {
            await _relaysController.PressGarageDoorRemoteButton();
        }
    }
}
