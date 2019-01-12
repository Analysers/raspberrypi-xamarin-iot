using Appliance.Controllers;
using Appliance.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Appliance.Commands
{
    public class ToggleAlarmStrobeHandler : IRequestHandler<ToggleAlarmStrobeCommand, StateResponse>
    {
        private readonly IRelaysController _relaysController;

        public ToggleAlarmStrobeHandler(IRelaysController relaysController)
        {
            _relaysController = relaysController;
        }

        public Task<StateResponse> Handle(ToggleAlarmStrobeCommand request, CancellationToken token)
        {
            return Task.FromResult(new StateResponse(_relaysController.AlarmStrobeToggle()));
        }
    }

    public class ToggleAlarmStrobeCommand : IRequest<StateResponse>
    {
    }
}
