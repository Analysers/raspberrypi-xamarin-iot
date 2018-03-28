using System.Threading.Tasks;
using Appliance.Controllers;
using Appliance.Models;
using MediatR;

namespace Appliance.Commands
{
    public class ToggleGardenLightsHandler : AsyncRequestHandler<ToggleGardenLightsCommand, StateResponse>
    {
        private readonly ILightsController _lightsController;

        public ToggleGardenLightsHandler(ILightsController lightsController)
        {
            _lightsController = lightsController;
        }

        protected override Task<StateResponse> HandleCore(ToggleGardenLightsCommand request)
        {
            return Task.FromResult(new StateResponse(_lightsController.GardenLightsToggle()));
        }
    }

    public class ToggleGardenLightsCommand : IRequest<StateResponse>
    {
    }
}
