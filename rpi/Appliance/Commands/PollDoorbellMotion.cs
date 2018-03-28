using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Appliance.Domain;
using Appliance.Enums;
using Appliance.Services;
using MediatR;

namespace Appliance.Commands
{
    public class PollDoorbellMotionHandler : INotificationHandler<PollDoorbellMotionCommand>
    {
        private readonly IRingService _ringService;
        private readonly IAlarmState _alarmState;

        public PollDoorbellMotionHandler(IRingService ringService, IAlarmState alarmState)
        {
            _ringService = ringService;
            _alarmState = alarmState;
        }

        public async Task Handle(PollDoorbellMotionCommand notification, CancellationToken cancellationToken)
        {
            async Task PollActiveDings()
            {
                var dings = await _ringService.PollActiveDings(cancellationToken);
                if (dings.Any(ding => ding.Kind.ToLower(CultureInfo.InvariantCulture) == "motion"))
                {
                    await _alarmState.Tripped(Sensor.FrontDoorMotion);
                }
            }

            await PollActiveDings();
        }
    }

    public class PollDoorbellMotionCommand : INotification
    {
    }
}
