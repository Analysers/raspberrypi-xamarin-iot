using System.Threading;
using System.Threading.Tasks;
using Appliance.Controllers;
using Appliance.Notifications;
using MediatR;

namespace Appliance.NotificationHandlers
{
    public class ActivateGarageDoor : INotificationHandler<PressGarageDoorRemoteButtonCommand>
    {
        private readonly IGarageController _garageController;

        public ActivateGarageDoor(IGarageController garageController)
        {
            _garageController = garageController;
        }

        public async Task Handle(PressGarageDoorRemoteButtonCommand notification, CancellationToken cancellationToken)
        {
            await _garageController.PressGarageDoorRemoteButton(cancellationToken);
        }
    }
}
