using System.Threading.Tasks;
using Appliance.Controllers;
using Appliance.Models;
using MediatR;

namespace Appliance.Commands
{
    public class ToggleAlarmSirenHandler : AsyncRequestHandler<ToggleAlarmSirenCommand, StateResponse>
    {
        private readonly IRelaysController _relaysController;

        public ToggleAlarmSirenHandler(IRelaysController relaysController)
        {
            _relaysController = relaysController;
        }

        protected override Task<StateResponse> HandleCore(ToggleAlarmSirenCommand request)
        {
            return Task.FromResult(new StateResponse(_relaysController.AlarmSirenToggle()));
        }
    }

    public class ToggleAlarmSirenCommand : IRequest<StateResponse>
    {
    }
}
