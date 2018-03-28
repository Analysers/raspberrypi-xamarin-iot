using System.Threading;
using System.Threading.Tasks;
using Appliance.Components;
using Appliance.Enums;
using Appliance.Events;
using Appliance.Notifications;
using MediatR;

namespace Appliance.NotificationHandlers
{
    public class GarageDoorOperated : INotificationHandler<PressGarageDoorRemoteButtonCommand>
    {
        private readonly ITimerEvents _timerEvents;
        private readonly ISensorBoard _sensorBoard;

        public GarageDoorOperated(ITimerEvents timerEvents, ISensorBoard sensorBoard)
        {
            _timerEvents = timerEvents;
            _sensorBoard = sensorBoard;
        }

        public async Task Handle(PressGarageDoorRemoteButtonCommand notification, CancellationToken cancellationToken)
        {
            async Task IfGarageDoorClosedResetOperatedConfig()
            {
                await Task.Delay(Config.GarageDoorOperationDuration, cancellationToken);
                if (!_sensorBoard.GarageDoorOpen())
                {
                    Config.GarageDoorOperated = false;
                    _timerEvents.DisableEvent(TimedEvent.GarageDoorOperatedEnd);
                }
            }

            Config.GarageDoorOperated = true;

            await IfGarageDoorClosedResetOperatedConfig();
        }
    }
}
