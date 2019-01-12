using Appliance.Controllers;
using Appliance.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Appliance.Commands
{
    public class ToggleAlarmSirenHandler : IRequestHandler<ToggleAlarmSirenCommand, StateResponse>
    {
        private readonly IRelaysController _relaysController;

        public ToggleAlarmSirenHandler(IRelaysController relaysController)
        {
            _relaysController = relaysController;
        }

        public Task<StateResponse> Handle(ToggleAlarmSirenCommand request, CancellationToken token)
        {
            return Task.FromResult(new StateResponse(_relaysController.AlarmSirenToggle()));
        }
    }

    public class ToggleAlarmSirenCommand : IRequest<StateResponse>
    {
    }
}
