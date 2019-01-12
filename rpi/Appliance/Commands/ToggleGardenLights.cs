using Appliance.Controllers;
using Appliance.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Appliance.Commands
{
    public class ToggleGardenLightsHandler : IRequestHandler<ToggleGardenLightsCommand, StateResponse>
    {
        private readonly ILightsController _lightsController;

        public ToggleGardenLightsHandler(ILightsController lightsController)
        {
            _lightsController = lightsController;
        }

        public Task<StateResponse> Handle(ToggleGardenLightsCommand request, CancellationToken token)
        {
            return Task.FromResult(new StateResponse(_lightsController.GardenLightsToggle()));
        }
    }

    public class ToggleGardenLightsCommand : IRequest<StateResponse>
    {
    }
}
