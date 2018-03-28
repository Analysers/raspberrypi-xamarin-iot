using System.Threading.Tasks;
using Appliance.Controllers;
using Appliance.Models;
using MediatR;

namespace Appliance.Commands
{
    public class ToggleAlarmStrobeHandler : AsyncRequestHandler<ToggleAlarmStrobeCommand, StateResponse>
    {
        private readonly IRelaysController _relaysController;

        public ToggleAlarmStrobeHandler(IRelaysController relaysController)
        {
            _relaysController = relaysController;
        }

        protected override Task<StateResponse> HandleCore(ToggleAlarmStrobeCommand request)
        {
            return Task.FromResult(new StateResponse(_relaysController.AlarmStrobeToggle()));
        }
    }

    public class ToggleAlarmStrobeCommand : IRequest<StateResponse>
    {
    }
}
